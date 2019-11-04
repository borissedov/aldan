using System.Collections.Generic;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial interface IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyToAddress">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="headers">Headers</param>
        void SendEmail(string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
             string replyToAddress = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null, 
            IDictionary<string, string> headers = null);
    }
}
