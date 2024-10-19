using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;

namespace Firefly_iii_pp.EventHandlers
{
    public interface ISingleEventHandler
    {
        public string EventName { get; }
        public Task<IComponent> HandleAsync(IRequestData requestData);

    }
}
