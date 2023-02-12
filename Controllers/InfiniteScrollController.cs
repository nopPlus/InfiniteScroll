using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
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

        public InfiniteScrollController(InfiniteScrollSettings pluginSettings,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            ICategoryService categoryService,
            CatalogSettings catalogSettings)
        {
            _pluginSettings = pluginSettings;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeContext = storeContext;
            _categoryService = categoryService;
            _catalogSettings = catalogSettings;
        }

        public virtual async Task<IActionResult> AllProducts()
        {
            return View();
        }


        public virtual async Task<IActionResult> LoadProducts(int categoryId = 0, int orderBy = 15, int pageSize = 0, int page = 1)
        {
            ViewBag.PageSize = pageSize;
            ViewBag.OrderBy = orderBy;

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
    }
}