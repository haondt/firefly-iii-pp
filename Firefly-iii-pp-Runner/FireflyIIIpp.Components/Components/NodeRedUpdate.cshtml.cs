using FireflyIIIpp.Components.Abstractions;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;
namespace FireflyIIIpp.Components.Components
{
    public class NodeRedUpdateModel : IPartialComponentModel
    {
        public Optional<string> ResponseText { get; set; } = new();
        public Optional<string> RequestText { get; set; } = new();

        public string ViewPath => NodeRedUpdateComponentDescriptorFactory.ViewPath;

        public bool IsSwap = true;
    }

    public class NodeRedUpdateComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public static readonly string ViewPath = "~/Components/NodeRedUpdate.cshtml";
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<NodeRedUpdateModel>
            {
                ViewPath = ViewPath
            };
        }
    }

}
