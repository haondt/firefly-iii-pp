using FireflyIIIpp.Components.Components;
using FireflyIIIpp.Core.Pages;
using Haondt.Web.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Services;

namespace FireflyIIIpp.Components.Pages
{
    public class PageComponentMap
    {
        public static Dictionary<string, Page> ComponentIdentities = new()
        {
            { ComponentDescriptor<NodeRedModel>.TypeIdentity, Page.NodeRed },
            { ComponentDescriptor<LookupModel>.TypeIdentity, Page.Lookup }
        };

        public static Dictionary<Page, Func<IPageComponentFactory, Task<IComponent<PageModel>>>> PageComponentFactories = new()
        {
            { Page.NodeRed, pf => pf.GetComponent<NodeRedModel>() },
            { Page.Lookup, pf => pf.GetComponent<LookupModel>() }
        };


        public static Dictionary<Page, string> PageFromComponenentIdentity = new()
        {
            { Page.NodeRed, ComponentDescriptor<NodeRedModel>.TypeIdentity },
            { Page.Lookup, ComponentDescriptor<LookupModel>.TypeIdentity }
        };

        public static Dictionary<Page, string> PageNames { get; } = new Dictionary<Page, string>
        {
            { Page.NodeRed, "Node-Red" },
            { Page.IntegrationTests, "Integration Tests" },
            { Page.Lookup, "Lookup" },
            { Page.FireflyIIIpp, "Firefly-III" },
            { Page.Reconcile, "Reconcile" },
        };

        public static Dictionary<Page, string> PathNames = new()
        {
            { Page.NodeRed, "node-red" },
            { Page.IntegrationTests, "integration-tests" },
            { Page.Lookup, "lookup" },
            { Page.FireflyIIIpp, "firefly-iii" },
            { Page.Reconcile, "reconcile" }
        };

        public static Dictionary<string, Page> PageFromPathName = new()
        {
            { "node-red", Page.NodeRed },
            { "integration-tests", Page.IntegrationTests },
            { "lookup", Page.Lookup },
            { "firefly-iii", Page.FireflyIIIpp },
            { "reconcile", Page.Reconcile }
        };
    }
}
