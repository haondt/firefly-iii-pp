namespace Haondt.Web.Exceptions
{
    public class ToastableException(ToastSeverity severity, Exception inner) : Exception
    {
        public ToastSeverity Severity { get; init; } = severity;
        public Exception Inner { get; init; } = inner;
    }

    public enum ToastSeverity
    {
        Debug,
        Info,
        Warning,
        Error
    }
}
