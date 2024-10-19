using Firefly_pp_Runner.Lookup.Services;
using FireflyIIIpp.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;

namespace Firefly_iii_pp.EventHandlers.Lookup
{
    public class DeleteForeignKeyEventHandler(
        ILookupStoreProvider storeProvider,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "DeleteForeignKey";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var foreignKey = requestData.Query.GetValue<string>("foreignKey");
            var storeName = requestData.Form.GetValue<string>("store");

            var store = storeProvider.GetStore(storeName);
            await store.DeleteForeignKey(foreignKey);

            return await componentFactory.GetPlainComponent(new ToastModel
            {
                Message = "deleted foreign key",
                Severity = ToastSeverity.Success
            }, configureResponse: m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                .TriggerAfterSettle("delete", string.Empty)
                .Build());
        }
    }
}
