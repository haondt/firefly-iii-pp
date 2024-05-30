using Haondt.Web.Services;

namespace Haondt.Web.Pages
{
    public class RegisteredPageEntryFactoryWrapper(IRegisteredPageEntryFactory inner, IPageRegistry pageRegistry) : IPageEntryFactory
    {
        public string Page => inner.Page;

        public string ViewPath => inner.ViewPath;

        public Task<PageEntry> Create(IRequestData data, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null) => inner.Create(pageRegistry, data, responseOptions);

        public Task<PageEntry> Create(IPageModel model, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null) => inner.Create(pageRegistry, model, responseOptions);
    }
}
