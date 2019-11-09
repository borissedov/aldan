using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aldan.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user model
    /// </summary>
    public partial class UserModel : BaseAldanEntityModel
    {
        #region Ctor

        public UserModel()
        {
            SendEmail = new SendEmailModel() { SendImmediately = true };

            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties
        
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Full name")]
        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //registration date
        [DisplayName("Created on")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Last activity date")]
        public DateTime LastActivityDate { get; set; }

        //IP address
        [DisplayName("IP Address")]
        public string LastIpAddress { get; set; }

        [DisplayName("Last visited page")]
        public string LastVisitedPage { get; set; }

        [DisplayName("User role")]
        public string RoleName { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [DisplayName("User role")]
        public int SelectedUserRoleId { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }

        #endregion

        #region Nested classes

        public partial class SendEmailModel : BaseAldanModel
        {
            [DisplayName("Subject")]
            public string Subject { get; set; }

            [DisplayName("Body")]
            public string Body { get; set; }

            [DisplayName("Send immediately")]
            public bool SendImmediately { get; set; }
        }

        #endregion
    }
}