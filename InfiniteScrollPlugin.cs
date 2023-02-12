using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace NopPlus.Plugin.InfiniteScroll
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class InfiniteScrollPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly InfiniteScrollSettings _pluginSettings;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public InfiniteScrollPlugin(ISettingService settingService,
            InfiniteScrollSettings pluginSettings,
            IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _pluginSettings = pluginSettings;
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widgSSE et zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { _pluginSettings.WidgetZone });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/InfiniteScrollAdmin/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "InfiniteScrollLink";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new InfiniteScrollSettings()
            {
                PageSize = 24,
                WidgetZone = "header_menu_after"
            };

            await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["NopPlus.Plugin.InfiniteScroll.PageTitle"] = "All Products",
                ["NopPlus.Plugin.InfiniteScroll.LinkTitle"] = "All Products",
                ["NopPlus.Plugin.InfiniteScroll.ContentEnd"] = "All products are loaded",
                ["NopPlus.Plugin.InfiniteScroll.ContentError"] = "Error loading products",

                ["NopPlus.Plugin.InfiniteScroll.Fields.PageSize"] = "Lazy Loading Page Size",
                ["NopPlus.Plugin.InfiniteScroll.Fields.PageSize.Hint"] = "Number of products load on each request.",
                ["NopPlus.Plugin.InfiniteScroll.Fields.WidgetZone"] = "The start of the showing special effect.",
                ["NopPlus.Plugin.InfiniteScroll.Fields.WidgetZone.Hint"] = "Widget zone to show infinit scroll page link.",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<InfiniteScrollSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("NopPlus.Plugin.InfiniteScroll");

            await base.UninstallAsync();
        }


        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}
