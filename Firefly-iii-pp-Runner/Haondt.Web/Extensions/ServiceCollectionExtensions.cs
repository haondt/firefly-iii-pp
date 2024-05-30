using Haondt.Web.Assets;
using Haondt.Web.Extensions;
using Haondt.Web.Filters;
using Haondt.Web.Liftetime;
using Haondt.Web.Pages;
using Haondt.Web.Persistence;
using Haondt.Web.Services;
using Haondt.Web.Styles;
using Haondt.Web.Views;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Haondt.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAssetServices();
            services.AddStylesServices(configuration);
            services.Configure<IndexSettings>(configuration.GetSection(nameof(IndexSettings)));

            services.AddSingleton<IPageRegistry, PageRegistry>();

            var indexSettings = configuration.GetSection(nameof(IndexSettings)).Get<IndexSettings>();
            services.RegisterPage("navigationBar", "~/Views/NavigationBar.cshtml", data =>
            {
                data.Query.TryGetValue(NavigationBarModel.CurrentViewKey, out string? castedValue);
                return new NavigationBarModel
                {
                    Pages = indexSettings!.NavigationBarPages.Select(p => (p, p.Equals(castedValue, StringComparison.OrdinalIgnoreCase))).ToList(),
                    Actions = []
                };
            });
            services.RegisterPage("loader", "~/Views/Loader.cshtml", () => throw new InvalidOperationException());
            services.RegisterPage("dynamicForm", "~/Views/DynamicForm.cshtml", () => throw new InvalidOperationException());
            services.RegisterPage("index", "~/Views/Index.cshtml", () => throw new InvalidOperationException());
            services.RegisterPage("toast", "~/Views/Toast.cshtml", () => throw new InvalidOperationException(),
                r => r.ReSwap("afterbegin").ReTarget("#toast-container"));
            services.RegisterPage("modal", "~/Views/Modal.cshtml", () => throw new InvalidOperationException(), r =>
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

        public static IServiceCollection AddAssetServices(this IServiceCollection services)
        {
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddSingleton<IAssetProvider, AssetProvider>();
            services.AddSingleton<IAssetSource, ManifestAssetSource>(_ => new ManifestAssetSource(typeof(ServiceCollectionExtensions).Assembly));
            return services;
        }

        public static IServiceCollection AddStylesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IStylesProvider, StylesProvider>();
            var assembly = typeof(ServiceCollectionExtensions).Assembly;
            services.AddSingleton<IStylesSource, ManifestAssetStylesSource>(sp => new ManifestAssetStylesSource(assembly, "base.css"));
            services.AddSingleton<IStylesSource, ManifestAssetStylesSource>(sp => new ManifestAssetStylesSource(assembly, "styles.css"));
            services.Configure<ColorSettings>(configuration.GetSection(nameof(ColorSettings)));
            services.AddSingleton<IStylesSource, ColorsStylesSource>();
            return services;
        }


        public static IServiceCollection RegisterPage(this IServiceCollection services,
            string page,
            string viewPath,
            Func<IPageRegistry, IRequestData, IPageModel> modelFactory,
            Func<HxHeaderBuilder, HxHeaderBuilder>? headerOptions = null)
        {

            services.AddSingleton<IRegisteredPageEntryFactory>(new DefaultPageEntryFactory(
                new DefaultPageEntryFactoryData
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
