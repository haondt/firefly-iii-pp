using FireflyIIIpp.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using System.Text.Json;

namespace Firefly_iii_pp.EventHandlers.NodeRed
{
    public class PrettifyRequestEventHandler(IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "PrettifyNodeRedRequest";

        public Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var lookupValue = requestData.Form.GetValue<string>("requestText");

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

            return componentFactory.GetPlainComponent(new NodeRedUpdateModel
            {
                RequestText = new(prettifiedJson)
            });
        }
    }
}
