using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Aldan.Core;
using Aldan.Core.Configuration;
using Aldan.Core.Domain.Customers;
using Aldan.Core.Domain.Messages;
using Aldan.Services.Customers;
using Aldan.Services.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly AldanConfig _config;

        private Dictionary<string, IEnumerable<string>> _allowedTokens;

        #endregion

        #region Ctor

        public MessageTokenProvider(
            IActionContextAccessor actionContextAccessor, ICustomerService customerService,
            IEventPublisher eventPublisher, IUrlHelperFactory urlHelperFactory, IWorkContext workContext,
            AldanConfig config)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _config = config;
        }

        #endregion

        #region Allowed tokens

        /// <summary>
        /// Get all available tokens by token groups
        /// </summary>
        protected Dictionary<string, IEnumerable<string>> AllowedTokens
        {
            get
            {
                if (_allowedTokens != null)
                    return _allowedTokens;

                _allowedTokens = new Dictionary<string, IEnumerable<string>>();

                //platform tokens
                _allowedTokens.Add(TokenGroupNames.FrameworkTokens, new[]
                {
                    "%Platform.Name%",
                    "%Platform.URL%",
                    "%Platform.Email%",
                    "%Platform.CompanyName%",
                    "%Platform.CompanyAddress%",
                    "%Platform.CompanyPhoneNumber%",
                    "%Platform.CompanyVat%",
                    "%Facebook.URL%",
                    "%Twitter.URL%",
                    "%YouTube.URL%"
                });

                //customer tokens
                _allowedTokens.Add(TokenGroupNames.CustomerTokens, new[]
                {
                    "%Customer.Email%",
                    "%Customer.FullName%",
                    "%Customer.FirstName%",
                    "%Customer.LastName%",
                    "%Customer.VatNumber%",
                    "%Customer.VatNumberStatus%",
                    "%Customer.CustomAttributes%",
                    "%Customer.PasswordRecoveryURL%",
                    "%Customer.AccountActivationURL%",
                    "%Customer.EmailRevalidationURL%",
                    "%Wishlist.URLForCustomer%"
                });

                //contact us tokens
                _allowedTokens.Add(TokenGroupNames.ContactUs, new[]
                {
                    "%ContactUs.SenderEmail%",
                    "%ContactUs.SenderName%",
                    "%ContactUs.Body%"
                });

                return _allowedTokens;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generates an absolute URL, routeName and route values
        /// </summary>
        /// <param name="routeName">The name of the route that is used to generate URL</param>
        /// <param name="routeValues">An object that contains route values</param>
        /// <returns>Generated URL</returns>
        protected virtual string RouteUrl(string routeName = null, object routeValues = null)
        {
            if (string.IsNullOrEmpty(_config.PlatformSettings.Url))
                throw new Exception("URL cannot be null");

            //generate a URL with an absolute path
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = new PathString(urlHelper.RouteUrl(routeName, routeValues));

            //remove the application path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
            url.StartsWithSegments(pathBase, out url);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{_config.PlatformSettings.Url.TrimEnd('/')}{url}"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add platform tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        public virtual void AddPlatformTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("Platform.Name", _config.PlatformSettings.Name));
            tokens.Add(new Token("Platform.URL", _config.PlatformSettings.Url, true));
            tokens.Add(new Token("Platform.Email",  _config.PlatformSettings.Email));
            tokens.Add(new Token("Platform.CompanyName",  _config.PlatformSettings.CompanyName));
            tokens.Add(new Token("Platform.CompanyAddress",  _config.PlatformSettings.CompanyAddress));
            tokens.Add(new Token("Platform.CompanyPhoneNumber",  _config.PlatformSettings.CompanyPhoneNumber));
            tokens.Add(new Token("Platform.CompanyVat",  _config.PlatformSettings.CompanyVat));

            tokens.Add(new Token("Facebook.URL",  _config.PlatformSettings.FacebookLink));
            tokens.Add(new Token("Twitter.URL",  _config.PlatformSettings.TwitterLink));
            tokens.Add(new Token("YouTube.URL",  _config.PlatformSettings.YoutubeLink));
        }


        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        public virtual void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            
            //note: we do not use SEO friendly URLS for these links because we can get errors caused by having .(dot) in the URL (from the email address)
            var passwordRecoveryUrl = RouteUrl(routeName: "PasswordRecoveryConfirm", routeValues: new { token = customer.PasswordRecoveryToken, email = customer.Email });
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
        }

        public IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups = null)
        {
            var allowedTokens = AllowedTokens.Where(x => tokenGroups == null || tokenGroups.Contains(x.Key))
                .SelectMany(x => x.Value).ToList();

            return allowedTokens.Distinct();
        }


        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public virtual IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            switch (messageTemplate.Name)
            {
                case MessageTemplateSystemNames.CustomerRegisteredNotification:
                case MessageTemplateSystemNames.CustomerWelcomeMessage:
                case MessageTemplateSystemNames.CustomerPasswordRecoveryMessage:
                    return new[] { TokenGroupNames.FrameworkTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.ContactUsMessage:
                    return new[] { TokenGroupNames.FrameworkTokens, TokenGroupNames.ContactUs };

                default:
                    return new string[] { };
            }
        }

        #endregion
    }
}