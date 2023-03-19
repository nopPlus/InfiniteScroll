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

        /// Gets a name of the route to the import contacts callback
        /// </summary>
        public static string InfiniteScrollPageRoute => "NopPlus.Plugin.InfiniteScroll.AllProducts";

    }
}