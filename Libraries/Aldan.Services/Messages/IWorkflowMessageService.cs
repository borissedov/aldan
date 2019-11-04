using System.Collections.Generic;
using Aldan.Core.Domain.Customers;
using Aldan.Core.Domain.Messages;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial interface IWorkflowMessageService
    {
        #region Customer workflow

        /// <summary>
        /// Sends 'New customer' notification message to a platform owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>Queued email identifier</returns>
        IList<int> SendCustomerRegisteredNotificationMessage(Customer customer);

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>Queued email identifier</returns>
        IList<int> SendCustomerWelcomeMessage(Customer customer);
        
        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <returns>Queued email identifier</returns>
        IList<int> SendCustomerPasswordRecoveryMessage(Customer customer);

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
        IList<int> SendContactUsMessage(string senderEmail, string senderName, string subject, string body);

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>Queued email identifier</returns>
        int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens);

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
        int SendNotification(
            MessageTemplate messageTemplate, IEnumerable<Token> tokens, 
            string toEmailAddress, string toName, 
            string replyToEmailAddress = null, string replyToName = null, 
            string fromEmail = null, string fromName = null, string subject = null);

        #endregion
    }
}