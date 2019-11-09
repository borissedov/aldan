using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Models.User
{
    public partial class PasswordRecoveryModel : BaseAldanModel
    {
        [DataType(DataType.EmailAddress)]
        [DisplayName("Account.PasswordRecovery.Email")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}