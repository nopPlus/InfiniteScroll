using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using NopPlus.Plugin.InfiniteScroll.Models;

namespace NopPlus.Plugin.InfiniteScroll.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class InfiniteScrollAdminController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;


        public InfiniteScrollAdminController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IDateTimeHelper dateTimeHelper)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var infiniteScrollSettings = await _settingService.LoadSettingAsync<InfiniteScrollSettings>(storeScope);

            var model = new ConfigurationModel()
            {
                PageSize = infiniteScrollSettings.PageSize,
                WidgetZone = infiniteScrollSettings.WidgetZone,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.PageSize_OverrideForStore = await _settingService.SettingExistsAsync(infiniteScrollSettings, x => x.PageSize, storeScope);
                model.WidgetZone_OverrideForStore = await _settingService.SettingExistsAsync(infiniteScrollSettings, x => x.WidgetZone, storeScope);
            }

            return View("~/Plugins/NopPlus.InfiniteScroll/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var infiniteScrollSettings = await _settingService.LoadSettingAsync<InfiniteScrollSettings>(storeScope);

            //save settings
            infiniteScrollSettings.PageSize = model.PageSize;
            infiniteScrollSettings.WidgetZone = model.WidgetZone;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(infiniteScrollSettings, x => x.PageSize, model.PageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(infiniteScrollSettings, x => x.WidgetZone, model.WidgetZone_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}