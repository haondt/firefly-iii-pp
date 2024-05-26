using Haondt.Web.Controllers;
using Haondt.Web.DynamicForm.Models;
using Haondt.Web.Exceptions;
using Haondt.Web.Pages;
using Microsoft.AspNetCore.Mvc;

namespace Haondt.Web.Services
{
    public interface IControllerHelper
    {
        public Task<IActionResult> GetModal(BaseController controller, PageEntry content, bool allowClickOut = false);
        public Task<IActionResult> GetModal(BaseController controller, string page, IDynamicFormFactory content, bool allowClickOut = false);
        public Task<IActionResult> GetModal(BaseController controller, IDynamicFormFactory content, bool allowClickOut = false);

        public Task<IActionResult> GetToastView(BaseController controller, List<(ToastSeverity Severity, string Message)> toasts);
        public Task<IActionResult> GetToastView(BaseController controller, ToastSeverity severity, string message);

        public Task<IActionResult> GetView(BaseController controller, string page, Func<Task<IPageModel>>? modelFactory = null, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);
        public Task<IActionResult> GetView(BaseController controller, string page, IDynamicFormFactory dynamicFormFactory, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);
        public Task<IActionResult> GetView(BaseController controller, IDynamicFormFactory dynamicFormFactory, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);
        public Task<IActionResult> GetView(BaseController controller, string page, Func<IPageModel> modelFactory, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null);

    }
}
