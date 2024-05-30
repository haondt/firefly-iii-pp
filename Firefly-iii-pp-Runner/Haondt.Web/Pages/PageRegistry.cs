using System.Diagnostics.CodeAnalysis;

namespace Haondt.Web.Pages
{
    public class PageRegistry(IEnumerable<IRegisteredPageEntryFactory> pageEntryFactories) : IPageRegistry
    {
        private readonly IReadOnlyDictionary<string, IRegisteredPageEntryFactory> _pageFactories = pageEntryFactories
                .GroupBy(f => f.Page, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(grp => grp.Key, grp => grp.Last(), StringComparer.OrdinalIgnoreCase);

        public bool TryGetPageFactory(string page, [NotNullWhen(true)] out IPageEntryFactory? entry)
        {
            if (!_pageFactories.TryGetValue(page, out var factory))
            {
                entry = null;
                return false;
            }

            entry = new RegisteredPageEntryFactoryWrapper(factory, this);
            return true;

        }

        public IPageEntryFactory GetPageFactory(string page)
            => new RegisteredPageEntryFactoryWrapper(_pageFactories[page], this);
    }
}
