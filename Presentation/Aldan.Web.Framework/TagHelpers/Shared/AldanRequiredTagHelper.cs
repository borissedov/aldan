using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Aldan.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// aldan-required tag helper
    /// </summary>
    [HtmlTargetElement("aldan-required", TagStructure = TagStructure.WithoutEndTag)]
    public class AldanRequiredTagHelper : TagHelper
    {
        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //clear the output
            output.SuppressOutput();

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "required");
            output.Content.SetContent("*");
        }
    }
}