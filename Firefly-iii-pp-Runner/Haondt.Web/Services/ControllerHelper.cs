using Haondt.Web.Controllers;
using Haondt.Web.DynamicForm.Models;
using Haondt.Web.Exceptions;
using Haondt.Web.Extensions;
using Haondt.Web.Pages;
using Haondt.Web.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haondt.Web.Services
{
    public class ControllerHelper(IPageRegistry pageRegistry, IOptions<IndexSettings> options) : IControllerHelper
    {
        private readonly IndexSettings _indexSettings = options.Value;

        public async Task<IActionResult> GetModal(BaseController controller, PageEntry content, bool allowClickOut = false)
        {
            var modal = await pageRegistry.GetPageFactory("modal").Create(new ModalModel
            {
                Content = content,
                AllowClickOut = allowClickOut
            });
            return modal.CreateView(controller);
        }

        public Task<IActionResult> GetModal(BaseController controller, IDynamicFormFactory content, bool allowClickOut = false) => GetModal(controller, "dynamicForm", content, allowClickOut);

        public async Task<IActionResult> GetModal(BaseController controller, string page, IDynamicFormFactory content, bool allowClickOut = false)
        {
            var modal = await pageRegistry.GetPageFactory("modal").Create(new ModalModel
            {
                Content = await pageRegistry.GetPageFactory(page).Create(content.Create()),
                AllowClickOut = allowClickOut
            });
            return modal.CreateView(controller);
        }

        public async Task<IActionResult> GetToastView(BaseController controller, List<(ToastSeverity Severity, string Message)> toasts)
        {
            controller.Response.StatusCode = 200;

            var pageEntryFactory = pageRegistry.GetPageFactory("toast");
            var pageEntry = await pageEntryFactory.Create(new ToastModel
            {
                Toasts = toasts
            });

            return pageEntry.CreateView(controller);
        }

        public Task<IActionResult> GetToastView(BaseController controller, ToastSeverity severity, string message) => GetToastView(controller, [(severity, message)]);




        public virtual async Task<IActionResult> GetView(BaseController controller, string page, Func<Task<IPageModel>>? modelFactory = null, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
        {
            if (!pageRegistry.TryGetPageFactory(page, out var pageEntryFactory))
            {
                pageEntryFactory = pageRegistry.GetPageFactory(_indexSettings.HomePage);
                modelFactory = null;
                responseOptions = null;
            }

            var pageEntry = modelFactory != null
                ? await pageEntryFactory.Create(await modelFactory(), responseOptions)
                : await pageEntryFactory.Create(controller.Request.AsRequestData(), responseOptions);

            return pageEntry.CreateView(controller);
        }

        public Task<IActionResult> GetView(BaseController controller, string page, IDynamicFormFactory dynamicFormFactory, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
            => GetView(controller, page, () => Task.FromResult<IPageModel>(dynamicFormFactory.Create()), responseOptions);
        public Task<IActionResult> GetView(BaseController controller, IDynamicFormFactory dynamicFormFactory, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
            => GetView(controller, "dynamicForm", () => Task.FromResult<IPageModel>(dynamicFormFactory.Create()), responseOptions);

        public Task<IActionResult> GetView(BaseController controller, string page, Func<IPageModel> modelFactory, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
            => GetView(controller, page, () => Task.FromResult(modelFactory()), responseOptions);
    }
}
