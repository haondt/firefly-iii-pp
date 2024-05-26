using System.Diagnostics.CodeAnalysis;

namespace Haondt.Web.Pages
{
    public class PageRegistry(IEnumerable<IPageEntryFactory> pageEntryFactories) : IPageRegistry
    {
        private readonly IReadOnlyDictionary<string, IPageEntryFactory> _pageFactories = pageEntryFactories
                .GroupBy(f => f.Page, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(grp => grp.Key, grp => grp.Last(), StringComparer.OrdinalIgnoreCase);

        public bool TryGetPageFactory(string page, [NotNullWhen(true)] out IPageEntryFactory? entry) 
            => _pageFactories.TryGetValue(page, out entry);

        public IPageEntryFactory GetPageFactory(string page) => _pageFactories[page];
    }
}
