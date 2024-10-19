using FireflyIIIpp.Components.Abstractions;
using Haondt.Core.Models;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class AutocompleteModel : IPartialComponentModel
    {
        public required string SuggestionEvent { get; set; }
        public Optional<string> Name { get; set; } = new();
        public Optional<string> Id { get; set; } = new();
        public Optional<string> Placeholder { get; set; } = new();
        public Optional<string> HxInclude { get; set; } = new();

        public string HxIncludeString
        {
            get
            {
                var result = "previous .input";
                if (HxInclude.HasValue)
                    result = $"{HxInclude.Value}, {result}";
                return result;
            }
        }


        public string ViewPath => AutocompleteComponentDescriptorFactory.ViewPath;
    }

    public class AutocompleteComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public static string ViewPath = "~/Components/Autocomplete.cshtml";
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<AutocompleteModel>
            {
                ViewPath = ViewPath
            };
        }
    }
}
