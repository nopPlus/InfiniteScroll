using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Web.Framework.Components;

namespace NopPlus.Plugin.InfiniteScroll.Components
{
    [ViewComponent(Name = PluginDefaults.INFINITESCROLL_LINK_VIEW_COMPONENT_NAME)]
    public class InfiniteScrollLinkViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly InfiniteScrollSettings _pluginSettings;
        #endregion

        #region Ctor

        public InfiniteScrollLinkViewComponent(IStoreContext storeContext,
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
            if (!_pluginSettings.TopMenuLink)
                return Content(string.Empty);

            return View();
        }

        #endregion
    }
}
