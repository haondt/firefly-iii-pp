using Firefly_iii_pp_Runner.Services;
using Firefly_iii_pp_Runner.Settings;
using Firefly_pp_Runner.Persistence;
using Firefly_pp_Runner.Services;
using Firefly_pp_Runner.Settings;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.AutoReconcile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly_pp_Runner.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFireflyIIIPPRunnerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ThunderClientEditorSettings>(configuration.GetSection(nameof(ThunderClientEditorSettings)));
            services.Configure<KeyValueStoreSettings>(configuration.GetSection(nameof(KeyValueStoreSettings)));

            services.AddSingleton<JobManager>();
            services.AddSingleton<ThunderClientEditorService>();
            services.AddSingleton<IAutoReconcileService, AutoReconcileService>();


            return services;
        }

        public static IServiceCollection AddFilePersistenceServices(this IServiceCollection services)
        {
            services.AddSingleton<IPersistenceService, FilePersistenceService>();
            return services;
        }
        public static IServiceCollection AddMemoryPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPersistenceService, MemoryPersistenceService>();
            return services;
        }
    }
}
