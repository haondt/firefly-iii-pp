using Haondt.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Haondt.Web.Controllers
{
    [Produces("text/html")]
    [ServiceFilter(typeof(ToastErrorFilter))]
    [ServiceFilter(typeof(ValidationFilter))]
    public class BaseController : Controller { }
}
