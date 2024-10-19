using FireflyIIIpp.Components.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FireflyIIIpp.Components.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static Task<IHtmlContent> PartialAsync(this IHtmlHelper html, IPartialComponentModel partialModel)
        {
            return html.PartialAsync(partialModel.ViewPath, partialModel);
        }
    }
}
