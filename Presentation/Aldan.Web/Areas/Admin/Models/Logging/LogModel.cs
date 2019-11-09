using System;
using System.ComponentModel;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents a log model
    /// </summary>
    public partial class LogModel : BaseAldanEntityModel
    {
        #region Properties

        [DisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [DisplayName("Admin.System.Log.Fields.ShortMessage")]
        public string ShortMessage { get; set; }

        [DisplayName("Admin.System.Log.Fields.FullMessage")]
        public string FullMessage { get; set; }

        [DisplayName("Admin.System.Log.Fields.IPAddress")]
        public string IpAddress { get; set; }

        [DisplayName("Admin.System.Log.Fields.User")]
        public int? UserId { get; set; }

        [DisplayName("Admin.System.Log.Fields.User")]
        public string UserEmail { get; set; }

        [DisplayName("Admin.System.Log.Fields.PageURL")]
        public string PageUrl { get; set; }

        [DisplayName("Admin.System.Log.Fields.ReferrerURL")]
        public string ReferrerUrl { get; set; }

        [DisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}