using Firefly_pp_Runner.Lookup.Services;
using FireflyIIIpp.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class LookupModel : IComponentModel
    {
        public List<string> Stores { get; set; } = [];
    }

    public class LookupComponentDescriptorFactory(ILookupStoreProvider lookupProvider) : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<LookupModel>(() => new()
            {
                Stores = lookupProvider.GetAvailableStores()
            })
            {
                ViewPath = $"~/Components/Lookup.cshtml",
            };
        }
    }
}
