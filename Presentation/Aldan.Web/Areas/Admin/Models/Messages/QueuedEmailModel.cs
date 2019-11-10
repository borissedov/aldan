using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a queued email model
    /// </summary>
    public partial class QueuedEmailModel: BaseAldanEntityModel
    {
        #region Properties

        [DisplayName("Id")]
        public override int Id { get; set; }

        [DisplayName("From")]
        public string From { get; set; }

        [DisplayName("From name")]
        public string FromName { get; set; }

        [DisplayName("To")]
        public string To { get; set; }

        [DisplayName("To name")]
        public string ToName { get; set; }

        [DisplayName("Reply to")]
        public string ReplyTo { get; set; }

        [DisplayName("Reply to name")]
        public string ReplyToName { get; set; }

        [DisplayName("CC")]
        public string CC { get; set; }

        [DisplayName("Bcc")]
        public string Bcc { get; set; }

        [DisplayName("Subject")]
        public string Subject { get; set; }

        [DisplayName("Body")]
        public string Body { get; set; }

       
        [DisplayName("Created on")]
        public DateTime CreatedOn { get; set; }
        

        [DisplayName("Sent tries")]
        public int SentTries { get; set; }

        [DisplayName("Sent on")]
        public DateTime? SentOn { get; set; }

        #endregion
    }
}