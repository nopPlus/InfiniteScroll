using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Web.Framework.Components;

namespace NopPlus.Plugin.InfiniteScroll.Components
{
    [ViewComponent(Name = "InfiniteScrollLink")]
    public class InfiniteScrollLinkViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        #endregion

        #region Ctor

        public InfiniteScrollLinkViewComponent(IStoreContext storeContext,
            IStaticCacheManager staticCacheManager)
        {
            _storeContext = storeContext;
            _staticCacheManager = staticCacheManager;
        }
        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return Content("");
        }

        #endregion
    }
}
