using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a queued email search model
    /// </summary>
    public partial class QueuedEmailSearchModel : BaseSearchModel
    {
        #region Properties

        [DisplayName("Start date")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [DisplayName("End date")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("From address")]
        public string SearchFromEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("To address")]
        public string SearchToEmail { get; set; }

        [DisplayName("Load not sent emails only")]
        public bool SearchLoadNotSent { get; set; }

        [DisplayName("Maximum send attempts")]
        public int SearchMaxSentTries { get; set; }

        [DisplayName("Go directly to email #")]
        public int GoDirectlyToNumber { get; set; }

        #endregion
    }
}