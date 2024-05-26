using Haondt.Web.Controllers;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haondt.Web.Authentication.Services
{
    public interface IAuthenticatedControllerHelper : IControllerHelper
    {
        public Task<IActionResult> GetForceLoginView(BaseController controller);
        public Task<(bool IsValid, IActionResult? InvalidSessionResponse)> VerifySession(BaseController controller);
    }
}
