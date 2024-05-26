using Microsoft.Extensions.Primitives;
using System.Collections;

namespace Haondt.Web
{
    public interface IRequestData
    {
        IFormCollection Form { get; }
        IQueryCollection Query { get; }
        IRequestCookieCollection Cookies { get; }
    }
}
