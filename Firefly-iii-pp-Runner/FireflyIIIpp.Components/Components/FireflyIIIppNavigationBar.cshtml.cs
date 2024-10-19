using FireflyIIIpp.Components.Abstractions;
using FireflyIIIpp.Core.Pages;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class FireflyIIIppNavigationBarModel : IComponentModel
    {
        public Page? ActivePage { get; set; }

    }

    public class NavigationBarComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<FireflyIIIppNavigationBarModel>(() => new())
            {
                ViewPath = $"~/Components/FireflyIIIppNavigationBar.cshtml",
            };
        }
    }
}
