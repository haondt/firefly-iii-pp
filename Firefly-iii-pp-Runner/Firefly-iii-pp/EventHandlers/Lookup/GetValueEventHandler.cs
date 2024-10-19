using Firefly_pp_Runner.Lookup.Reasons;
using Firefly_pp_Runner.Lookup.Services;
using FireflyIIIpp.Components.Components;
using FireflyIIIpp.Core.Exceptions;
using Haondt.Web.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;

namespace Firefly_iii_pp.EventHandlers.Lookup
{
    public class GetValueEventHandler(
        ILookupStoreProvider storeProvider,
        IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "GetLookupValue";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var storeName = requestData.Form.TryGetValue<string>("store");
            if (!storeName.HasValue)
                throw new UserException("Please select a store");
            var primaryKey = requestData.Form.GetValue<string>("primaryKey");

            var store = storeProvider.GetStore(storeName.Value);
            var value = await store.GetValueAsync(primaryKey);
            if (!value.IsSuccessful)
            {
                if (value.Reason == LookupResultReason.NotFound)
                    return await componentFactory.GetPlainComponent(new AppendComponentLayoutModel
                    {
                        Components = new List<IComponent>
                        {
                            await componentFactory.GetPlainComponent(new LookupUpdateModel
                            {
                                LookupValue = new("")
                            }),
                            await componentFactory.GetPlainComponent(new ToastModel
                            {
                                Message = "key has no associated value",
                                Severity = ToastSeverity.Info
                            })
                        }
                    });

                throw new ArgumentException($"Failed to retrieve value due to reason {value.Reason}");
            }

            return await componentFactory.GetPlainComponent(new AppendComponentLayoutModel
            {
                Components = new List<IComponent>
                {
                    await componentFactory.GetPlainComponent(new LookupUpdateModel
                    {
                        LookupValue = new(value.Value)
                    }),
                    await componentFactory.GetPlainComponent(new ToastModel
                    {
                        Message = "retrieved value successfully",
                        Severity = ToastSeverity.Success
                    })
                }
            });
        }
    }
}
