using Aldan.Core;
using Aldan.Services.Common;
using Aldan.Web.Areas.Admin.Models.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays the setting mode
    /// </summary>
    public class SettingModeViewComponent : ViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public SettingModeViewComponent(
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="modeName">Setting mode name</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string modeName = "settings-advanced-mode")
        {
            //prepare model
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = _genericAttributeService.GetAttribute<bool>(_workContext.CurrentUser, modeName)
            };
            
            return View(model);
        }

        #endregion
    }
}