using System.Diagnostics.CodeAnalysis;

namespace Haondt.Web.Pages
{
    public interface IPageRegistry
    {

        public bool TryGetPageFactory(string page, [NotNullWhen(true)] out IPageEntryFactory? entry);

        public IPageEntryFactory GetPageFactory(string page);
    }
}
