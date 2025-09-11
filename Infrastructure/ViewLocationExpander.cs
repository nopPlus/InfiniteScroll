using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Web.Framework;

namespace NopPlus.Plugin.InfiniteScroll.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            viewLocations = new[] {
                "/Plugins/NopPlus.InfiniteScroll/Views/Shared/{0}.cshtml",
                "/Plugins/NopPlus.InfiniteScroll/Views/InfiniteScroll/{0}.cshtml",
            }.Concat(viewLocations);
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.AreaName == AreaNames.ADMIN)
                return;
        }
    }
}
