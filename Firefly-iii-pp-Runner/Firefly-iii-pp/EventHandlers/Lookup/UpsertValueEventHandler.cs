using Firefly_pp_Runner.Lookup.Services;
using FireflyIIIpp.Components.Components;
using FireflyIIIpp.Core.Exceptions;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;

namespace Firefly_iii_pp.EventHandlers.Lookup
{
    public class UpsertValueEventHandler(
        ILookupStoreProvider storeProvider,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "UpsertLookupValue";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var storeName = requestData.Form.TryGetValue<string>("store");
            if (!storeName.HasValue)
                throw new UserException("Please select a store");
            var primaryKey = requestData.Form.GetValue<string>("primaryKey");
            var lookupValue = requestData.Form.GetValue<string>("lookupValue");

            var store = storeProvider.GetStore(storeName.Value);
            await store.UpsertPrimaryKeyValue(primaryKey, lookupValue);

            return await componentFactory.GetPlainComponent(new ToastModel
            {
                Message = "updated value",
                Severity = ToastSeverity.Success
            });
        }
    }
}
