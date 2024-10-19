using FireflyIIIpp.Components.Abstractions;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;
namespace FireflyIIIpp.Components.Components
{
    public class NodeRedUpdateModel : IComponentModel
    {
        public Optional<string> ResponseText { get; set; } = new();
        public Optional<string> ErrorMessage { get; set; } = new();
    }

    public class NodeRedUpdateComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<NodeRedUpdateModel>
            {
                ViewPath = $"~/Components/NodeRedUpdate.cshtml",
            };
        }
    }

}
