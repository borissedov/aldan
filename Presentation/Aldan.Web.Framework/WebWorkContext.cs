using System;
using Aldan.Core;
using Aldan.Core.Domain.Users;
using Aldan.Core.Http;
using Aldan.Services.Authentication;
using Aldan.Services.Users;
using Microsoft.AspNetCore.Http;

namespace Aldan.Web.Framework
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private User _cachedUser;
        private User _originalUserIfImpersonated;

        #endregion

        #region Ctor

        public WebWorkContext(
            IAuthenticationService authenticationService,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get Aldan user cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetUserCookie()
        {
            var cookieName = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.UserCookie}";
            return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set Aldan user cookie
        /// </summary>
        /// <param name="userGuid">Guid of the user</param>
        protected virtual void SetUserCookie(Guid userGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.UserCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (userGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, userGuid.ToString(), options);
        }
       

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public virtual User CurrentUser
        {
            get
            {
                //whether there is a cached value
                if (_cachedUser != null)
                    return _cachedUser;
                      
                //try to get registered user
                User user = _authenticationService.GetAuthenticatedUser();

                if (user != null && !user.Deleted)
                {
                    //get impersonate user if required
                    var impersonatedUserId = user.ImpersonatedUserId;
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = _userService.GetUserById(impersonatedUserId.Value);
                        if (impersonatedUser != null && !impersonatedUser.Deleted)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }

                if (user == null || user.Deleted)
                {
                    //get guest user
                    var userCookie = GetUserCookie();
                    if (!string.IsNullOrEmpty(userCookie))
                    {
                        if (Guid.TryParse(userCookie, out Guid userGuid))
                        {
                            //get user from cookie (should not be registered)
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null)
                                user = userByCookie;
                        }
                    }
                }

                if (user != null && !user.Deleted)
                {
                    //set user cookie
                    SetUserCookie(user.UserGuid);

                    //cache the found user
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.UserGuid);
                _cachedUser = value;
            }
        }

        /// <summary>
        /// Gets the original user (in case the current one is impersonated)
        /// </summary>
        public virtual User OriginalUserIfImpersonated
        {
            get { return _originalUserIfImpersonated; }
        }

        #endregion
    }
}