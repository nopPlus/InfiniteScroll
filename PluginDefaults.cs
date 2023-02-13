using Nop.Core;
using Nop.Core.Caching;

namespace NopPlus.Plugin.InfiniteScroll
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class PluginDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "NopPlus.InfiniteScroll";

        /// <summary>
        /// Gets the key for caching tax rate
        /// </summary>
        /// <remarks>
        /// {0} - Store id
        /// {1} - Language id
        /// {2} - Customer Roles
        /// {3} - Entity Id
        /// {4} - WidgetZone
        /// </remarks>
        //public static CacheKey RelatedProductCacheKey => new ("Products.{0}-{1}-{2}-{3}-{4}", PrefixCacheKey);

        /// <summary>
        /// Gets the prefix key to clear cache
        /// </summary>
       // public static string PrefixCacheKey => "NopPlus.Plugin.SmartRelatedProducts";

        /// <summary>
        /// Name of the view component to display widget in public store
        /// </summary>
        public const string INFINITESCROLL_LINK_VIEW_COMPONENT_NAME = "InfiniteScrollLink";

        /// <summary>
        /// Name of the view component to display widget in public store
        /// </summary>
       // public const string PRODUCT_VIEW_COMPONENT_NAME = "NopPlus.SmartRelatedProducts.ProductViewed";

        /// <summary>
        /// Gets a name of the route to the import contacts callback
        /// </summary>
        public static string InfiniteScrollPageRoute => "NopPlus.Plugin.InfiniteScroll.AllProducts";

        /// <summary>
        /// Cache time for auto assign cycle
        /// </summary>
        //public static int AutoAssignCycleCacheTime => 30;

    }
}