using FireflyIIIpp.Components.Abstractions;
using FireflyIIIpp.Components.Pages;
using FireflyIIIpp.Core.Pages;
using Haondt.Web.Core.Components;
using Haondt.Web.Services;

namespace FireflyIIIpp.Components.Components
{
    public class FireflyIIIppLayoutModel : IComponentModel
    {
        public required IComponent Content { get; set; }
        public required IComponent NavigationBar { get; set; }
    }

    public class FireflyIIIppLayoutComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<FireflyIIIppLayoutModel>()
            {
                ViewPath = $"~/Components/FireflyIIIppLayout.cshtml",
            };
        }
    }

    public class FireflyIIIppLayoutComponentFactory(IComponentFactory componentFactory) : ILayoutComponentFactory
    {

        public async Task<IComponent> GetLayoutAsync(IComponent content, string componentIdentity)
        {
            Page? activePage = PageComponentMap.ComponentIdentities.TryGetValue(componentIdentity, out var page)
                ? page : null;

            var navigationBar = await componentFactory.GetPlainComponent(new FireflyIIIppNavigationBarModel
            {
                ActivePage = page
            });
            return await componentFactory.GetPlainComponent(new FireflyIIIppLayoutModel
            {
                Content = content,
                NavigationBar = navigationBar
            });
        }
    }
}
