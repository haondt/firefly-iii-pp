using Haondt.Web.Exceptions;
using Haondt.Web.Pages;

namespace Haondt.Web.Views
{
    public class ToastModel : IPageModel
    {
        public required List<(ToastSeverity Severity, string Message)> Toasts { get; set; }
    }

}
