using FireflyIIIpp.Components.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;
using Haondt.Web.Core.Services;

namespace Firefly_iii_pp.EventHandlers
{
    public class NavigateEventHandler(IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "Navigate";

        public Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var target = requestData.Query.GetValue<string>("target");
            if (target == "lookup")
            {
                return componentFactory.GetPlainComponent<LookupModel>(configureResponse: m =>
                    m.ConfigureHeadersAction = new HxHeaderBuilder()
                        .PushUrl("lookup")
                        .ReTarget("#content")
                        .ReSwap("innerHTML")
                        .Build());
            }
            else if (target == "node-red")
            {
                return componentFactory.GetPlainComponent<NodeRedModel>(configureResponse: m =>
                    m.ConfigureHeadersAction = new HxHeaderBuilder()
                        .PushUrl("node-red")
                        .ReTarget("#content")
                        .ReSwap("innerHTML")
                        .Build());
            }

            throw new InvalidOperationException($"Unknown target {target}");
        }
    }
}
