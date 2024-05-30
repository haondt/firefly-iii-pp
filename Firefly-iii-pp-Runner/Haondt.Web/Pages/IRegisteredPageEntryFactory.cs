using Haondt.Web.Services;

namespace Haondt.Web.Pages
{
    public interface IRegisteredPageEntryFactory
    {
        public string Page { get; }

        public string ViewPath { get; }

        public Task<PageEntry> Create(IPageRegistry pageRegistry, IRequestData data, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);

        public Task<PageEntry> Create(IPageRegistry pageRegistry, IPageModel model, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);
    }
}
