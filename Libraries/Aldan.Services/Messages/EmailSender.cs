using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Aldan.Core.Configuration;

namespace Aldan.Services.Messages
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial class EmailSender : IEmailSender
    {
        private readonly AldanConfig _config;

        #region Fields


        #endregion

        #region Ctor

        public EmailSender(AldanConfig config)
        {
            _config = config;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="headers">Headers</param>
        public virtual void SendEmail(string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
             string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            IDictionary<string, string> headers = null)
        {
            var message = new MailMessage
            {
                //from, to, reply to
                From = new MailAddress(fromAddress, fromName)
            };
            message.To.Add(new MailAddress(toAddress, toName));
            if (!string.IsNullOrEmpty(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            //headers
            if (headers != null)
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = _config.EmailSettings.Host;
                smtpClient.Port = _config.EmailSettings.Port;
                smtpClient.EnableSsl = _config.EmailSettings.EnableSsl;
                smtpClient.Credentials =
                    new NetworkCredential(_config.EmailSettings.Username, _config.EmailSettings.Password);
                smtpClient.Send(message);
            }
        }

        #endregion
    }
}