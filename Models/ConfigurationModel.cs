using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopPlus.Plugin.InfiniteScroll.Models
{
    public partial record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("NopPlus.Plugin.InfiniteScroll.Fields.PageSize")]
        public int PageSize { get; set; }
        public bool PageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("NopPlus.Plugin.InfiniteScroll.Fields.TopMenuLink")]
        public bool TopMenuLink { get; set; }
        public bool TopMenuLink_OverrideForStore { get; set; }
    }
}