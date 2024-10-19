using Firefly_pp_Runner.Lookup.Services;
using FireflyIIIpp.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;

namespace Firefly_iii_pp.EventHandlers.Lookup
{
    public class ForeignKeyAutocompleteEventHandler(
        ILookupStoreProvider storeProvider,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "ForeignKeyAutocomplete";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var foreignKey = requestData.Form.TryGetValue<string>("foreignKey");
            var storeName = requestData.Form.TryGetValue<string>("store");
            if (!storeName.HasValue)
                return await componentFactory.GetPlainComponent(new AutocompleteSuggestionsModel
                {
                    Items = []
                });

            var store = storeProvider.GetStore(storeName.Value);
            var suggestions = foreignKey.HasValue
                ? await store.AutocompleteForeignKey(foreignKey.Value, 5) // todo
                : await store.AutocompleteForeignKey("", 5);

            return await componentFactory.GetPlainComponent(new AutocompleteSuggestionsModel
            {
                Items = suggestions
            }, configureResponse: m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                .ReSwap("innerHTML")
                .Build());
        }
    }
}
