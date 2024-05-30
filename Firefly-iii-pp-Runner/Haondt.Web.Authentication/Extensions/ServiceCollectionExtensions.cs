using Haondt.Web.Authentication.Pages;
using Haondt.Web.Authentication.Services;
using Haondt.Web.Extensions;
using Haondt.Web.Pages;
using Haondt.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haondt.Web.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterPage("login", "~/Views/DynamicForm.cshtml", new LoginDynamicFormFactory("").Create, r => r.ConfigureForPage("login"));
            services.RegisterPage("register", "~/Views/DynamicForm.cshtml", new RegisterDynamicFormFactory("").Create, r => r.ConfigureForPage("register"));
            services.RegisterAuthenticatedPage("dynamicFormWithAuthentication", "~/Views/DynamicForm.cshtml", () => throw new InvalidOperationException());

            services.Configure<AuthenticationSettings>(configuration.GetSection(nameof(AuthenticationSettings)));
            services.AddScoped<AuthenticationService>();
            services.AddSingleton<CryptoService>();
            services.AddSingleton<UserService>();
            services.AddScoped<ISessionService, SessionService>();

            return services;
        }

        public static IServiceCollection RegisterAuthenticatedPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IPageRegistry, IRequestData, IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null)
        {
            services.AddSingleton<IRegisteredPageEntryFactory>(new NeedsAuthenticationDefaultPageEntryFactory(
                new DefaultPageEntryFactoryData
                {
                    Page = page,
                    ViewPath = viewPath,
                    ModelFactory = modelFactory,
                    ConfigureResponse = headerOptions
                }));
            return services;
        }

        public static IServiceCollection RegisterAuthenticatedPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null) => services.RegisterAuthenticatedPage(page, viewPath, (_, _) => modelFactory(), headerOptions);

        public static IServiceCollection RegisterAuthenticatedPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IRequestData, IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null) => services.RegisterAuthenticatedPage(page, viewPath, (_, data) => modelFactory(data), headerOptions);
    }
}
