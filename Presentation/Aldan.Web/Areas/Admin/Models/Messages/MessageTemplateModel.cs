using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aldan.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a message template model
    /// </summary>
    public partial class MessageTemplateModel : BaseAldanEntityModel
    {
        #region Properties

        [DisplayName("Allowed tokens")]
        public string AllowedTokens { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Bcc email addresses")]
        public string BccEmailAddresses { get; set; }

        [DisplayName("Subject")]
        public string Subject { get; set; }

        [DisplayName("Body")]
        public string Body { get; set; }

        [DisplayName("Is active")]
        public bool IsActive { get; set; }
        
        #endregion
    }
}