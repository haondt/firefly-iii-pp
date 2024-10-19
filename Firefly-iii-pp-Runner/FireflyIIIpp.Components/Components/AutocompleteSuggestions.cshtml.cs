using FireflyIIIpp.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class AutocompleteSuggestionsModel : IComponentModel
    {
        public List<string> Items { get; set; } = [];
    }

    public class AutocompleteSuggestionsComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public static string ViewPath = "~/Components/AutocompleteSuggestions.cshtml";
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<AutocompleteSuggestionsModel>(() => new())
            {
                ViewPath = ViewPath
            };
        }
    }
}
