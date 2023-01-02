using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Firefly_iii_pp_Runner.API.ExceptionFilters
{
    public abstract class AbstractExceptionFilter : ExceptionFilterAttribute
    {
        protected readonly Type _exceptionType;
        private readonly int _code;
        private readonly string _message;

        public AbstractExceptionFilter(Type exceptionType, int code, string message = null)
        {
            _exceptionType = exceptionType;
            _code = code;
            _message = message;
        }

        protected abstract bool CheckExceptionType(Type exceptionType);

        public override void OnException(ExceptionContext context)
        {
            if (CheckExceptionType(context.Exception.GetType()))
                context.Result = new ObjectResult(_message ?? context.Exception.ToString()) { StatusCode = _code };
            base.OnException(context);
        }
    }

}
