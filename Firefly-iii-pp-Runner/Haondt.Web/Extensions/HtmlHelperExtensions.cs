using Haondt.Web.Pages;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Haondt.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static Task<IHtmlContent> PartialAsync(this IHtmlHelper html, PageEntry content)
        {
            return content.PartialAsync(html);
        }
    }
}
