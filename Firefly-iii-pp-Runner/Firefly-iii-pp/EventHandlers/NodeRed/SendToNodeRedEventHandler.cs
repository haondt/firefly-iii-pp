using FireflyIIIpp.Components.Components;
using FireflyIIIpp.NodeRed.Reasons;
using FireflyIIIpp.NodeRed.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Http;

namespace Firefly_iii_pp.EventHandlers.NodeRed
{
    public class SendToNodeRedEventHandler(INodeRedService nodeRed, IComponentFactory componentFactory) : ISingleEventHandler
    {
        public string EventName => "SendToNodeRed";

        public async Task<IComponent> HandleAsync(IRequestData requestData)
        {
            var body = requestData.Form.TryGetValue<string>("requestText");
            if (!body.HasValue)
                throw new InvalidOperationException("missing `requestText` parameter in form");

            var result = await nodeRed.ApplyRules(body.Value);

            if (!result.IsSuccessful)
                return await componentFactory.GetPlainComponent(new NodeRedUpdateModel
                {
                    ResponseText = new(),
                    ErrorMessage = new(result.Reason)
                });

            if (result.Value.IsSuccessful)
                return await componentFactory.GetPlainComponent(new NodeRedUpdateModel
                {
                    ResponseText = new(result.Value.Value),
                    ErrorMessage = new("")
                });

            if (result.Value.Reason == ApplyRulesReason.NotModified)
                return await componentFactory.GetPlainComponent(new NodeRedUpdateModel
                {
                    ErrorMessage = new("")
                });

            throw new InvalidOperationException($"Unexpected {nameof(ApplyRulesReason)}: {result.Value.Reason}");
        }
    }
}
