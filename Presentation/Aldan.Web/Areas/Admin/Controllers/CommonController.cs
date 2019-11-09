using Aldan.Core;
using Aldan.Core.Caching;
using Aldan.Web.Areas.Admin.Factories;
using Aldan.Web.Areas.Admin.Models.Common;
using Aldan.Web.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public class CommonController : BaseAdminController
    {
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICommonModelFactory _commonModelFactory;

        public CommonController(
            IStaticCacheManager cacheManager,
            IWebHelper webHelper, 
            ICommonModelFactory commonModelFactory)
        {
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _commonModelFactory = commonModelFactory;
        }

        public virtual IActionResult SystemInfo()
        {
            //prepare model
            var model = _commonModelFactory.PrepareSystemInfoModel(new SystemInfoModel());

            return View(model);
        }
        
        [HttpPost]
        public virtual IActionResult ClearCache(string returnUrl = "")
        {
            _cacheManager.Clear();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            return Redirect(returnUrl);
        }

        [HttpPost]
        public virtual IActionResult RestartApplication(string returnUrl = "")
        {
            //restart application
            _webHelper.RestartAppDomain();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            return Redirect(returnUrl);
        }
    }
}