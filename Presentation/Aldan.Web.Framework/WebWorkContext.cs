using System;
using Aldan.Core;
using Aldan.Core.Domain.Customers;
using Aldan.Core.Http;
using Aldan.Services.Authentication;
using Aldan.Services.Customers;
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
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;

        #endregion

        #region Ctor

        public WebWorkContext(
            IAuthenticationService authenticationService,
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authenticationService = authenticationService;
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get Aldan customer cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetCustomerCookie()
        {
            var cookieName = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.CustomerCookie}";
            return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set Aldan customer cookie
        /// </summary>
        /// <param name="customerGuid">Guid of the customer</param>
        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = $"{AldanCookieDefaults.Prefix}{AldanCookieDefaults.CustomerCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (customerGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, customerGuid.ToString(), options);
        }
       

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual Customer CurrentCustomer
        {
            get
            {
                //whether there is a cached value
                if (_cachedCustomer != null)
                    return _cachedCustomer;
                      
                //try to get registered user
                Customer customer = _authenticationService.GetAuthenticatedCustomer();

                if (customer != null && !customer.Deleted)
                {
                    //get impersonate user if required
                    var impersonatedCustomerId = customer.ImpersonatedCustomerId;
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                if (customer == null || customer.Deleted)
                {
                    //get guest customer
                    var customerCookie = GetCustomerCookie();
                    if (!string.IsNullOrEmpty(customerCookie))
                    {
                        if (Guid.TryParse(customerCookie, out Guid customerGuid))
                        {
                            //get customer from cookie (should not be registered)
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null)
                                customer = customerByCookie;
                        }
                    }
                }

                if (customer != null && !customer.Deleted)
                {
                    //set customer cookie
                    SetCustomerCookie(customer.CustomerGuid);

                    //cache the found customer
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        public virtual Customer OriginalCustomerIfImpersonated
        {
            get { return _originalCustomerIfImpersonated; }
        }

        #endregion
    }
}