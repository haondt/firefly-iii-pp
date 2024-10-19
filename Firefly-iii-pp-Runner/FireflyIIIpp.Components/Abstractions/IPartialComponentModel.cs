using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Abstractions
{
    public interface IPartialComponentModel : IComponentModel
    {
        string ViewPath { get; }
    }
}
