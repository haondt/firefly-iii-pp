using FireflyIIIpp.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using System.Text.Json;

namespace Firefly_iii_pp.EventHandlers.Lookup
{
    public class PrettifyValueEventHandler(IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "PrettifyLookupValue";

        public Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var lookupValue = requestData.Form.GetValue<string>("lookupValue");

            string prettifiedJson;
            try
            {
                var jsonDocument = JsonDocument.Parse(lookupValue);
                prettifiedJson = JsonSerializer.Serialize(jsonDocument.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }
            catch
            {
                prettifiedJson = lookupValue;
            }

            return componentFactory.GetPlainComponent(new LookupUpdateModel
            {
                LookupValue = new(prettifiedJson)
            });
        }
    }
}
