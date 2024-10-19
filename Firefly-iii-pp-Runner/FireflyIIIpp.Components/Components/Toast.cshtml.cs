using FireflyIIIpp.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Components
{
    public class ToastModel : IComponentModel
    {
        public required string Message { get; set; }
        public ToastSeverity Severity { get; set; } = ToastSeverity.Error;
        public string SeverityString => Severity switch
        {
            ToastSeverity.Warning => "is-warning",
            ToastSeverity.Error => "is-danger",
            ToastSeverity.Success => "is-success",
            ToastSeverity.Info => "is-info",
            _ => throw new ArgumentException(Severity.ToString())
        };

    }

    public enum ToastSeverity
    {
        Error,
        Warning,
        Info,
        Success,
    }

    public class ToastComponentDescriptorFactory : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<ToastModel>
            {
                ViewPath = $"~/Components/Toast.cshtml"
            };
        }
    }
}
