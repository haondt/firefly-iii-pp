using FireflyIIIpp.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class EmptyComponentModel : IComponentModel
    {
    }

    public class EmptyComponentComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<EmptyComponentModel>(new EmptyComponentModel())
            {
                ViewPath = "~/Components/EmptyComponent.cshtml"
            };
        }
    }
}
