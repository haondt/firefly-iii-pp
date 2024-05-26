using Haondt.Web.Exceptions;

namespace Haondt.Web.Extensions
{
    public static class ExceptionExtensions
    {
        public static ToastableException Toast(this Exception exception, ToastSeverity severity) => new ToastableException(severity, exception);
    }
}
