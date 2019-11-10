using Aldan.Web.Framework.Models;

namespace Aldan.Web.Models.Common
{
    public partial class AdminHeaderLinksModel : BaseAldanModel
    {
        public string ImpersonatedUserName { get; set; }
        public bool IsUserImpersonated { get; set; }
        public bool DisplayAdminLink { get; set; }
    }
}