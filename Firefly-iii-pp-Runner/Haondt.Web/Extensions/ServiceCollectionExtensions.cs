using Haondt.Web.Extensions;
using Haondt.Web.Filters;
using Haondt.Web.Liftetime;
using Haondt.Web.Pages;
using Haondt.Web.Persistence;
using Haondt.Web.Services;
using Haondt.Web.Views;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;

namespace Haondt.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.Configure<AssetSettings>(configuration.GetSection(nameof(AssetSettings)));
            services.AddSingleton<AssetProvider>();
            services.Configure<StyleSettings>(configuration.GetSection(nameof(StyleSettings)));
            services.AddSingleton<StylesProvider>();
            services.Configure<IndexSettings>(configuration.GetSection(nameof(IndexSettings)));

            services.AddScoped<IPageRegistry, PageRegistry>();

            var indexSettings = configuration.GetSection(nameof(IndexSettings)).Get<IndexSettings>();
            services.RegisterPage("navigationBar", "~/Core/Views/NavigationBar.cshtml", data =>
            {
                data.Query.TryGetValue(NavigationBarModel.CurrentViewKey, out string? castedValue);
                return new NavigationBarModel
                {
                    Pages = indexSettings!.NavigationBarPages.Select(p => (p, p.Equals(castedValue, StringComparison.OrdinalIgnoreCase))).ToList(),
                    Actions = []
                };
            });
            services.RegisterPage("loader", "~/Core/Views/Loader.cshtml", () => throw new InvalidOperationException());
            services.RegisterPage("dynamicForm", "~/Core/Views/DynamicForm.cshtml", () => throw new InvalidOperationException());
            services.RegisterPage("index", "~/Core/Views/Index.cshtml", () => throw new InvalidOperationException());
            services.RegisterPage("toast", "~/Core/Views/Toast.cshtml", () => throw new InvalidOperationException(),
                r => r.ReSwap("afterbegin").ReTarget("#toast-container"));
            services.RegisterPage("modal", "~/Core/Views/Modal.cshtml", () => throw new InvalidOperationException(), r =>
                r.ReSwap("innerHTML").ReTarget("#modal-container"));


            services.AddSingleton<FileExtensionContentTypeProvider>();
            services.AddHttpContextAccessor();
            services.AddScoped<LifetimeHookService>();

            services.AddSingleton<IStorage, MemoryStorage>();
            services.Configure<PersistenceSettings>(configuration.GetSection(nameof(PersistenceSettings)));
            services.AddSingleton<IStorageService, StorageService>();

            services.AddScoped<ToastResponseService>();
            services.AddScoped<ToastErrorFilter>();
            services.AddScoped<ValidationFilter>();

            services.AddScoped<IControllerHelper, ControllerHelper>();

            return services;
        }

        public static IServiceCollection RegisterPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IPageRegistry, IRequestData, IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null)
        {

            services.AddSingleton<IPageEntryFactory>(sp 
                => ActivatorUtilities.CreateInstance<DefaultPageEntryFactory>(sp, new DefaultPageEntryFactoryData
                {
                    Page = page,
                    ViewPath = viewPath,
                    ModelFactory = modelFactory,
                    ConfigureResponse = headerOptions
                }));
            return services;
        }


        public static IServiceCollection RegisterPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null) => services.RegisterPage(page, viewPath, (_, _) => modelFactory(), headerOptions);

        public static IServiceCollection RegisterPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IRequestData, IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null) => services.RegisterPage(page, viewPath, (_, data) => modelFactory(data), headerOptions);
    }
}
