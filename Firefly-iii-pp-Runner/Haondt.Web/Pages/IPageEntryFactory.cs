using Haondt.Web.Services;

namespace Haondt.Web.Pages
{
    public interface IPageEntryFactory
    {
        public string Page { get; }

        public string ViewPath { get; }

        public Task<PageEntry> Create(IRequestData data, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);

        public Task<PageEntry> Create(IPageModel model, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);
    }
}
