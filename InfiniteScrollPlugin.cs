using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using NopPlus.Plugin.InfiniteScroll.Components;

namespace NopPlus.Plugin.InfiniteScroll
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class InfiniteScrollPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly WidgetSettings _widgetSettings;
        private readonly InfiniteScrollSettings _pluginSettings;

        public InfiniteScrollPlugin(ISettingService settingService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            WidgetSettings widgetSettings,
            InfiniteScrollSettings pluginSettings)
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;
            _widgetSettings = widgetSettings;
            _pluginSettings = pluginSettings;
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
            var widgetZones = new List<string> { PublicWidgetZones.CategoryDetailsBottom };
            if (_pluginSettings.TopMenuLink)
                widgetZones.Add(PublicWidgetZones.Footer);

            return Task.FromResult<IList<string>>(widgetZones);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/InfiniteScrollAdmin/Configure";
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (string.Equals(widgetZone, PublicWidgetZones.CategoryDetailsBottom))
                return typeof(InfiniteScrollCategoryViewComponent);
                
            return typeof(InfiniteScrollLinkViewComponent);
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
                PageSize = 12,
                TopMenuLink = true
            };
            await _settingService.SaveSettingAsync(settings);

            //enable widget
            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(PluginDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["NopPlus.Plugin.InfiniteScroll.PageTitle"] = "All Products",
                ["NopPlus.Plugin.InfiniteScroll.LinkTitle"] = "All Products",
                ["NopPlus.Plugin.InfiniteScroll.ContentEnd"] = "All products are loaded",
                ["NopPlus.Plugin.InfiniteScroll.ContentError"] = "Error loading products",

                ["NopPlus.Plugin.InfiniteScroll.Fields.PageSize"] = "Lazy Loading Page Size",
                ["NopPlus.Plugin.InfiniteScroll.Fields.PageSize.Hint"] = "Number of products load on each request.",
                ["NopPlus.Plugin.InfiniteScroll.Fields.TopMenuLink"] = "Show in Top Menu",
                ["NopPlus.Plugin.InfiniteScroll.Fields.TopMenuLink.Hint"] = "Infinite scroll page link display on top menu if enabled.",
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

            //disable widget
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(PluginDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

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
