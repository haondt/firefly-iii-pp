using FireflyIIIpp.Components.Abstractions;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class LookupUpdateModel : IPartialComponentModel
    {
        public bool IsSwap = true;
        public required Optional<string> LookupValue { get; set; } = new();
        public string ViewPath => LookupUpdateComponentDescriptorFactory.ViewPath;
    }

    public class LookupUpdateComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public static readonly string ViewPath = $"~/Components/LookupUpdate.cshtml";
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<LookupUpdateModel>
            {
                ViewPath = ViewPath
            };
        }
    }
}
