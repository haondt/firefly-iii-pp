namespace Firefly_iii_pp_Runner.ExceptionFilters
{
    public class InheritableExceptionFilter : AbstractExceptionFilter
    {
        public InheritableExceptionFilter(Type exceptionType, int code, string message = null) : base(exceptionType, code, message) { }
        protected override bool CheckExceptionType(Type exceptionType) => _exceptionType.IsAssignableFrom(exceptionType);
    }
}
