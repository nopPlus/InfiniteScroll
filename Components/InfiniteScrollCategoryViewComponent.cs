using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace NopPlus.Plugin.InfiniteScroll.Components
{
    [ViewComponent(Name = PluginDefaults.INFINITESCROLL_CATEGORY_COMPONENT_NAME)]
    public class InfiniteScrollCategoryViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly InfiniteScrollSettings _pluginSettings;
        #endregion

        #region Ctor

        public InfiniteScrollCategoryViewComponent(IStoreContext storeContext,
            IStaticCacheManager staticCacheManager,
            InfiniteScrollSettings pluginSettings)
        {
            _storeContext = storeContext;
            _staticCacheManager = staticCacheManager;
            _pluginSettings = pluginSettings;
        }
        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            ////if (!_pluginSettings.TopMenuLink)
            //    return Content(string.Empty);

            var model = (CategoryModel)additionalData;

            return View(model);
        }

        #endregion
    }
}
