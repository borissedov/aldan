using System.Collections.Generic;
using System.ComponentModel;
using Aldan.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aldan.Web.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user search model
    /// </summary>
    public partial class UserSearchModel : BaseSearchModel
    {
        #region Ctor

        public UserSearchModel()
        {
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [DisplayName("User roles")] public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }
        [DisplayName("Email")] public string SearchEmail { get; set; }
        [DisplayName("First name")] public string SearchFirstName { get; set; }
        [DisplayName("Last name")] public string SearchLastName { get; set; }
        [DisplayName("IP address")] public string SearchIpAddress { get; set; }

        #endregion
    }
}