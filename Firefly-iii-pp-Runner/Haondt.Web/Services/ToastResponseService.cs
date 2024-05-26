﻿using Haondt.Web.Exceptions;
using Haondt.Web.Pages;
using Haondt.Web.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Haondt.Web.Services
{
    public class ToastResponseService(IPageRegistry pageRegistry)
    {
        public async Task Overwrite(List<(ToastSeverity Severity, string Message)> toasts, HttpContext httpContext, Action<IActionResult> setActionResult)
        {
            httpContext.Response.Headers.Clear();
            httpContext.Response.StatusCode = 200;

            var pageEntryFactory = pageRegistry.GetPageFactory("Toast");
            var pageEntry = await pageEntryFactory.Create(new ToastModel
            {
                Toasts = toasts
            });

            var result = pageEntry.CreateView(httpContext.Response.Headers);
            setActionResult(result);
        }

        public Task Overwrite(ToastSeverity severity, string message, HttpContext httpContext, Action<IActionResult> setActionResult)
        {
            return Overwrite([(severity, message)], httpContext, setActionResult);
        }

    }
}
