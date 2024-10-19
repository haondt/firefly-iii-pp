using Firefly_pp_Runner.Lookup.Services;
using FireflyIIIpp.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;

namespace Firefly_iii_pp.EventHandlers.Lookup
{
    public class MapForeignKeyEventHandler(
        ILookupStoreProvider storeProvider,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "MapForeignKey";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var foreignKey = requestData.Form.GetValue<string>("foreignKey");
            var primaryKey = requestData.Form.GetValue<string>("primaryKey");
            var storeName = requestData.Form.GetValue<string>("store");

            var store = storeProvider.GetStore(storeName);
            await store.AddForeignKey(primaryKey, foreignKey);

            return await componentFactory.GetPlainComponent(new ToastModel
            {
                Message = "added map",
                Severity = ToastSeverity.Success
            });
        }
    }
}
