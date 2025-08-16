using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Web.Controllers;
using Nop.Web.Factories;

namespace NopPlus.Plugin.InfiniteScroll.Controllers
{
    public class InfiniteScrollController : BasePublicController
    {
        private readonly InfiniteScrollSettings _pluginSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly CatalogSettings _catalogSettings;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;

        public InfiniteScrollController(InfiniteScrollSettings pluginSettings,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            ICategoryService categoryService,
            CatalogSettings catalogSettings,
            ISpecificationAttributeService specificationAttributeService,
            IWorkContext workContext,
            ICurrencyService currencyService)
        {
            _pluginSettings = pluginSettings;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeContext = storeContext;
            _categoryService = categoryService;
            _catalogSettings = catalogSettings;
            _specificationAttributeService = specificationAttributeService;
            _workContext = workContext;
            _currencyService = currencyService;
        }

        #region Utilities

        /// <summary>
        /// Gets the price range converted to primary store currency
        /// </summary>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the <see cref="Task"/> containing the price range converted to primary store currency
        /// </returns>
        protected virtual async Task<(decimal? from, decimal? to)> GetConvertedPriceRangeAsync(string price)
        {
            //var result = new PriceRangeModel();
            decimal? priceFrom = null;
            decimal? priceTo = null;

            if (string.IsNullOrWhiteSpace(price))
                return (priceFrom, priceTo);

            var fromTo = price.Trim().Split(new[] { '-' });
            if (fromTo.Length == 2)
            {
                var rawFromPrice = fromTo[0]?.Trim();
                if (!string.IsNullOrEmpty(rawFromPrice) && decimal.TryParse(rawFromPrice, out var from))
                    priceFrom = from;

                var rawToPrice = fromTo[1]?.Trim();
                if (!string.IsNullOrEmpty(rawToPrice) && decimal.TryParse(rawToPrice, out var to))
                    priceTo = to;

                if (priceFrom > priceTo)
                    priceFrom = priceTo;

                var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

                if (priceFrom.HasValue)
                    priceFrom = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(priceFrom.Value, workingCurrency);

                if (priceTo.HasValue)
                    priceTo = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(priceTo.Value, workingCurrency);
            }

            return (priceFrom, priceTo);
        }

        #endregion

        public virtual async Task<IActionResult> AllProducts()
        {
            return View();
        }


        public virtual async Task<IActionResult> LoadProducts(int categoryId = 0, int orderBy = 15, int pageSize = 0, int page = 1)
        {
            ViewBag.PageSize = pageSize;
            ViewBag.OrderBy = orderBy;

            if (categoryId > 0)
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId);
                if (pageSize <= 0)
                {
                    //page size
                    pageSize = PreparePageSizeOptions(category.AllowCustomersToSelectPageSize, category.PageSizeOptions, category.PageSize);
                }
            }

            if (pageSize == 0)
                pageSize = _pluginSettings.PageSize;


            var productSortingEnum = (ProductSortingEnum)orderBy;

            ViewBag.CategoryId = 0;
            var categoryIds = new List<int>();
            if (categoryId > 0)
            {
                categoryIds.Add(categoryId);
                //model.CategoryId = categoryId;
                ViewBag.CategoryId = categoryId;
            }

            var storeId = _storeContext.GetCurrentStore().Id;

            var products = await _productService.SearchProductsAsync(
                categoryIds: categoryIds,
                storeId: storeId,
                visibleIndividuallyOnly: true,
                orderBy: productSortingEnum,
                pageIndex: page - 1,
                pageSize: pageSize);

            if (products.HasNextPage)
            {
                ViewBag.NextPage = page + 1;
            }

            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            return View("InfiniteProductGrid", model);

        }

        public virtual async Task<IActionResult> AjaxLoad(int categoryId,
            string viewmode = "",
            int? orderby = null,
            int? pagesize = null,
            int? page = null,
            string specs = "",
            string ms = "",
            string price = "")
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null || category.Deleted)
                return Content("");

            //set the order by position by default
            var searchOrderBy = ProductSortingEnum.Position;
            if (orderby != null && orderby > -1)
            {
                searchOrderBy = (ProductSortingEnum)orderby.Value;
            }
            else
            {
                //get active sorting options
                var activeSortingOptionsIds = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                    .Except(_catalogSettings.ProductSortingEnumDisabled).ToList();

                //order sorting options
                var orderedActiveSortingOptions = activeSortingOptionsIds
                    .Select(id => new { Id = id, Order = _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(id, out var order) ? order : id })
                    .OrderBy(option => option.Order).ToList();

                searchOrderBy = (ProductSortingEnum)orderedActiveSortingOptions.FirstOrDefault().Id;
            }

            //set the page size by default
            var searchPageSize = category.PageSize;
            if (pagesize != null && pagesize > -1)
            {
                searchPageSize = pagesize.Value;
            }

            //set page
            var searchPage = 1;
            if (page != null && page > -1)
            {
                searchPage = page.Value;
            }

            //set category id
            var categoryIds = new List<int>();
            if (categoryId > 0)
            {
                categoryIds.Add(categoryId);
            }

            //set view mode by default
            var viewMode = !string.IsNullOrEmpty(viewmode)
                ? viewmode
                : _catalogSettings.DefaultViewMode;


            //price range
            decimal? searchPriceMin = null;
            decimal? searchPriceMax = null;
            if (_catalogSettings.EnablePriceRangeFiltering && category.PriceRangeFiltering)
            {
                (searchPriceMin, searchPriceMax) = await GetConvertedPriceRangeAsync(price);
            }

            //filterable manufacturers
            var manufacturerIds = new List<int>();
            if (_catalogSettings.EnableManufacturerFiltering)
            {
                int msId = 0;
                manufacturerIds = ms?.Split(',')
                    .Select(m => { int.TryParse(m, out msId); return msId; })
                    .Where(m => m != 0).ToList();
            }

            //filterable options
            var filteredSpecs = new List<SpecificationAttributeOption>();
            if (_catalogSettings.EnableSpecificationAttributeFiltering)
            {
                var filterableOptions = await _specificationAttributeService
                    .GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(category.Id);

                int specId = 0;
                var specificationOptionIds = specs?.Split(',')
                    .Select(m => { int.TryParse(m, out specId); return specId; })
                    .Where(m => m != 0).ToList();

                filteredSpecs = specificationOptionIds is null ? null : filterableOptions.Where(fo => specificationOptionIds.Contains(fo.Id)).ToList();
            }

            //filter store
            var storeId = _storeContext.GetCurrentStore().Id;

            var products = await _productService.SearchProductsAsync(
                pageIndex: searchPage - 1,
                pageSize: searchPageSize,
                categoryIds: categoryIds,
                manufacturerIds: manufacturerIds,
                storeId: storeId,
                visibleIndividuallyOnly: true,
                priceMin: searchPriceMin,
                priceMax: searchPriceMax,
                filteredSpecOptions: filteredSpecs,
                orderBy: searchOrderBy);
            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            ViewBag.PageSize = searchPageSize;
            ViewBag.OrderBy = (int)searchOrderBy;
            ViewBag.CategoryId = category.Id;
            ViewBag.ViewMode = viewMode;

            if (products.HasNextPage)
            {
                ViewBag.NextPage = page + 1;
            }

            return View("InfiniteProductGrid", model);

        }

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private int PreparePageSizeOptions(bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            var pageSize = 0;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    var pageSizeOptionsValues = new List<string>();
                    foreach (var pageSizeStr in pageSizes)
                    {
                        if (!int.TryParse(pageSizeStr, out var temp))
                            continue;

                        if (temp <= 0)
                            continue;

                        pageSizeOptionsValues.Add(pageSizeStr);
                    }

                    if (pageSizeOptionsValues.Any())
                    {
                        pageSizeOptionsValues = pageSizeOptionsValues.OrderBy(x => int.Parse(x)).ToList();

                        if (pageSize <= 0)
                            pageSize = int.Parse(pageSizeOptionsValues.First());
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                pageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (pageSize <= 0)
            {
                pageSize = fixedPageSize;
            }

            return pageSize;
        }
    }
}