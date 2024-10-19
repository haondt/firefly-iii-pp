using FireflyIIIpp.Components.Abstractions;
using FireflyIIIpp.Components.Components;
using FireflyIIIpp.Components.Services;
using Haondt.Web.BulmaCSS.Extensions;
using Haondt.Web.Services;

namespace FireflyIIIpp.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddComponent<T>(this IServiceCollection services) where T : IComponentDescriptorFactory
        {
            return services.AddScoped(sp => ActivatorUtilities.CreateInstance<T>(sp).Create());
        }

        public static IServiceCollection AddFireflyIIIppComponentServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBulmaCSSHeadEntries();
            services.AddBulmaCSSComponents(configuration);
            services.AddBulmaCSSAssetSources();

            services.AddScoped<ILayoutComponentFactory, FireflyIIIppLayoutComponentFactory>();
            services.AddSingleton<ISingletonComponentFactory, SingletonComponentFactory>();

            return services;
        }

        public static IServiceCollection AddFireflyIIIppComponents(this IServiceCollection services)
        {
            services.AddCoreComponents();
            return services;
        }

        private static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services.AddComponent<NodeRedComponentDescriptorFactory>();
            services.AddComponent<NodeRedUpdateComponentDescriptorFactory>();
            services.AddComponent<FireflyIIIppLayoutComponentDescriptorFactory>();
            services.AddComponent<NavigationBarComponentDescriptorFactory>();
            services.AddComponent<LookupComponentDescriptorFactory>();
            services.AddComponent<LookupUpdateComponentDescriptorFactory>();
            services.AddComponent<ToastComponentDescriptorFactory>();
            services.AddComponent<AutocompleteComponentDescriptorFactory>();
            services.AddComponent<AutocompleteSuggestionsComponentDescriptorFactory>();
            services.AddComponent<EmptyComponentComponentDescriptorFactory>();
            return services;
        }
    }
}
