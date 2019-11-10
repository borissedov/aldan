using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Aldan.Core;
using Aldan.Core.Caching;
using Aldan.Core.Domain.Users;
using Aldan.Services.Common;
using Aldan.Services.Users;
using Aldan.Web.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Aldan.Web.Factories
{
    /// <summary>
    /// Represents the common models factory
    /// </summary>
    public partial class CommonModelFactory : ICommonModelFactory
    {
        #region Fields

        
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;

        #endregion

        #region Ctor

        public CommonModelFactory(
            IActionContextAccessor actionContextAccessor,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            IStaticCacheManager cacheManager,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper,
            IWorkContext workContext, IUserService userService)
        {
            _actionContextAccessor = actionContextAccessor;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
            _workContext = workContext;
            _userService = userService;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        public virtual AdminHeaderLinksModel PrepareAdminHeaderLinksModel()
        {
            var user = _workContext.CurrentUser;

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedUserName = user != null ? user.Email : "",
                IsUserImpersonated = _workContext.OriginalUserIfImpersonated != null,
                DisplayAdminLink = _workContext.CurrentUser.Role == Role.Admin
            };

            return model;
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        public virtual ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentUser.Email;
                model.FullName = _userService.GetUserFullName(_workContext.CurrentUser);
            }
            model.SubjectEnabled = true;
            model.DisplayCaptcha = false;

            return model;
        }

        #endregion
    }
}