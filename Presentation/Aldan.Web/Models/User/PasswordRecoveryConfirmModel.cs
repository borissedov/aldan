﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Models.User
{
    public partial class PasswordRecoveryConfirmModel : BaseAldanModel
    {
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string ConfirmNewPassword { get; set; }

        public bool DisablePasswordChanging { get; set; }
        public string Result { get; set; }
    }
}