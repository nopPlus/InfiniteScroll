using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopPlus.Plugin.InfiniteScroll.Models
{
    public partial record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("NopPlus.Plugin.InfiniteScroll.Fields.PageSize")]
        public int PageSize { get; set; }
        public bool PageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("NopPlus.Plugin.InfiniteScroll.Fields.WidgetZone")]
        public string WidgetZone { get; set; }
        public bool WidgetZone_OverrideForStore { get; set; }
    }
}