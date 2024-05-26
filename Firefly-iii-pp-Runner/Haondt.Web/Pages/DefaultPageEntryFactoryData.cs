using Haondt.Web.Services;

namespace Haondt.Web.Pages
{
    public class DefaultPageEntryFactoryData
    {
        public required string Page { get; init; }
        public required string ViewPath { get; init; }
        public required Func<IPageRegistry, IRequestData, IPageModel> ModelFactory { get; init; }
        public Func<HxHeaderBuilder, HxHeaderBuilder>? ConfigureResponse { get; init; }
    }
}
