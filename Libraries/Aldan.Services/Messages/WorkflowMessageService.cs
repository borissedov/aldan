using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Aldan.Core;
using Aldan.Core.Configuration;
using Aldan.Core.Domain.Customers;
using Aldan.Core.Domain.Messages;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields
        
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ITokenizer _tokenizer;
        private readonly AldanConfig _config;

        #endregion

        #region Ctor

        public WorkflowMessageService(
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IQueuedEmailService queuedEmailService,
            ITokenizer tokenizer, AldanConfig config)
        {
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _queuedEmailService = queuedEmailService;
            _tokenizer = tokenizer;
            _config = config;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get active message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <returns>List of message templates</returns>
        protected virtual IList<MessageTemplate> GetActiveMessageTemplates(string messageTemplateName)
        {
            //get message templates by the name
            var messageTemplates = _messageTemplateService.GetMessageTemplatesByName(messageTemplateName);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<MessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        #endregion

        #region Methods

        #region Customer workflow

        /// <summary>
        /// Sends 'New customer' notification message to a platform owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendCustomerRegisteredNotificationMessage(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerRegisteredNotification);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddPlatformTokens(tokens);

                var toEmail = _config.PlatformSettings.Email;
                var toName = _config.PlatformSettings.Name;

                return SendNotification(messageTemplate, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendCustomerWelcomeMessage(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerWelcomeMessage);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddPlatformTokens(tokens);

                var toEmail = customer.Email;
                var toName = customer.Email;// = _customerService.GetCustomerFullName(customer);

                return SendNotification(messageTemplate, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendCustomerPasswordRecoveryMessage(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerPasswordRecoveryMessage);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddPlatformTokens(tokens);

                var toEmail = customer.Email;
                var toName = customer.Email;//= _customerService.GetCustomerFullName(customer);

                return SendNotification(messageTemplate, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion


        #region Misc

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendContactUsMessage(string senderEmail,
            string senderName, string subject, string body)
        {
            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ContactUsMessage);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName),
                new Token("ContactUs.Body", body, true)
            };

            return messageTemplates.Select(messageTemplate =>
            {
                
                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddPlatformTokens(tokens);

                var fromEmail = _config.PlatformSettings.Email;
                    var fromName = _config.PlatformSettings.Name;
                    body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
                    
                var toEmail = _config.PlatformSettings.Email;
                var toName = _config.PlatformSettings.Name;

                return SendNotification(messageTemplate,tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName);
            }).ToList();
        }

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate == null)
                throw new ArgumentException("Template cannot be loaded");

            return SendNotification(messageTemplate, tokens, sendToEmail, null);
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNotification(MessageTemplate messageTemplate,IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));
            
            //retrieve localized message template data
            var bcc = messageTemplate.BccEmailAddresses;
            if (string.IsNullOrEmpty(subject))
                subject = messageTemplate.Subject;
            var body = messageTemplate.Body;

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : _config.PlatformSettings.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : _config.PlatformSettings.Name,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                CreatedOnUtc = DateTime.UtcNow
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        #endregion

        #endregion
    }
}