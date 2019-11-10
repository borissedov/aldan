using System.Collections.Generic;
using System.ComponentModel;
using Aldan.Web.Framework.Models;

namespace Aldan.Web.Areas.Admin.Models.Messages
{
    public partial class TestMessageTemplateModel : BaseAldanEntityModel
    {
        public TestMessageTemplateModel()
        {
            Tokens = new List<string>();
        }
        
        [DisplayName("Tokens")]
        public List<string> Tokens { get; set; }

        [DisplayName("Send email to")]
        public string SendTo { get; set; }
    }
}