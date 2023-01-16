using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Firefly_iii_pp_Runner.API.ExceptionFilters
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
                var result = new Dictionary<string, object>
                {
                    { "StatusCode",  _code},
                    { "Exception", context.Exception.GetType().Name },
                };
                if (!string.IsNullOrEmpty(_message))
                    result.Add("Message", _message);
                if (!string.IsNullOrEmpty(context.Exception.Message))
                    result.Add("Details", context.Exception.Message);

                context.Result = new ObjectResult(result) { StatusCode = _code };
            }

            base.OnException(context);
        }
    }
}
