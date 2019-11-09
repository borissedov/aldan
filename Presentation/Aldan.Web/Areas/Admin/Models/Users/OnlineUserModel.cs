using System;
using System.ComponentModel;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents an online user model
    /// </summary>
    public partial class OnlineUserModel : BaseAldanEntityModel
    {
        #region Properties

        [DisplayName("User info")]
        public string UserInfo { get; set; }

        [DisplayName("IP Address")]
        public string LastIpAddress { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("Last activity")]
        public DateTime LastActivityDate { get; set; }
        
        [DisplayName("Last visited page")]
        public string LastVisitedPage { get; set; }

        #endregion
    }
}