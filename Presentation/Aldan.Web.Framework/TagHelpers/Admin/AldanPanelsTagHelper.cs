using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Aldan.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// aldan-panel tag helper
    /// </summary>
    [HtmlTargetElement("aldan-panels", Attributes = ID_ATTRIBUTE_NAME)]
    public class AldanPanelsTagHelper : TagHelper
    {
        private const string ID_ATTRIBUTE_NAME = "id";

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}