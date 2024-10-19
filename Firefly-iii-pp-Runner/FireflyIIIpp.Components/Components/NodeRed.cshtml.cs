using FireflyIIIpp.Components.Abstractions;
using Haondt.Web.Core.Components;
namespace FireflyIIIpp.Components.Components
{
    public class NodeRedModel : IComponentModel
    {
    }

    public class NodeRedComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<NodeRedModel>(() => new())
            {
                ViewPath = $"~/Components/NodeRed.cshtml",
            };
        }
    }
}
