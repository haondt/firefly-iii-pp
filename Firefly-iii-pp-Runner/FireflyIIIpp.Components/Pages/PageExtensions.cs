using FireflyIIIpp.Core.Pages;

namespace FireflyIIIpp.Components.Pages
{
    public static class PageExtensions
    {
        public static string GetStringName(this Page page)
        {
            return PageComponentMap.PathNames[page];
        }
    }
}
