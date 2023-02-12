using Nop.Core.Configuration;

namespace NopPlus.Plugin.InfiniteScroll
{
    public class InfiniteScrollSettings : ISettings
    {
        public int PageSize { get; set; }

        public string WidgetZone { get; set; }

    }
}