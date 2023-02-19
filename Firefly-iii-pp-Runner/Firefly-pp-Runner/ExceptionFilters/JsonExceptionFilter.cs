using FireflyIIIpp.Core.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Firefly_iii_pp_Runner.ExceptionFilters
{
    public class JsonExceptionFilter : ExceptionFilterAttribute
    {
        protected readonly Type _exceptionType;
        protected readonly int _code;
        protected readonly string _message;

        public JsonExceptionFilter(Type exceptionType, int code, string message = null)
        {
            _exceptionType = exceptionType;
            _code = code;
            _message = message;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() == _exceptionType)
            {
                var result = new ExceptionDto
                {
                    StatusCode = _code,
                    Exception = context.Exception.GetType().Name,
                };
                if (!string.IsNullOrEmpty(_message))
                    result.Message = _message;
                if (!string.IsNullOrEmpty(context.Exception.Message))
                    result.Details = context.Exception.Message;

                context.Result = new ObjectResult(result) { StatusCode = _code };
            }

            base.OnException(context);
        }
    }
}
