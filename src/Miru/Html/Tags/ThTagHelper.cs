using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Miru.Html.Tags
{
    [HtmlTargetElement("miru-th")]
    public class ThTagHelper : MiruTagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var th = GetHtmlTag(nameof(HtmlConvention.TableHeader));
            
            var childContent = await output.GetChildContentAsync();
            
            if (childContent.IsEmptyOrWhiteSpace)
            {
                var span = HtmlGenerator.TagFor(For, nameof(HtmlConvention.DisplayLabels));
                    // .Id(GetId());
                
                th.Children.Add(span);
            }
            else
            {
                th.AppendHtml(childContent.GetContent());
                // th.Id(GetId());
                
                childContent.Clear();
            }

            SetOutput(th, output);
        }
    }
}