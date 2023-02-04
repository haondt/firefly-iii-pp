namespace Firefly_iii_pp_Runner.ExceptionFilters
{
    public class ExceptionFilter : AbstractExceptionFilter
    {
        public ExceptionFilter(Type exceptionType, int code, string message = null) : base(exceptionType, code, message) { }
        protected override bool CheckExceptionType(Type exceptionType) => exceptionType == _exceptionType;
    }
}
