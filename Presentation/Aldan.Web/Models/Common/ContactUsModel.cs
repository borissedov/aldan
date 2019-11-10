using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Models.Common
{
    public partial class ContactUsModel : BaseAldanModel
    {
        [DataType(DataType.EmailAddress)]
        [DisplayName("Your email")]
        public string Email { get; set; }
        
        [DisplayName("Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [DisplayName("Enquiry")]
        public string Enquiry { get; set; }

        [DisplayName("Your name")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}