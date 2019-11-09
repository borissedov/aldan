using System;
using Aldan.Core;
using Aldan.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public partial class PreferencesController : BaseAdminController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PreferencesController(IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult SavePreference(string name, bool value)
        {
            //permission validation is not required here
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _genericAttributeService.SaveAttribute(_workContext.CurrentUser, name, value);

            return Json(new
            {
                Result = true
            });
        }

        #endregion
    }
}