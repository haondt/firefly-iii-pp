using Firefly_iii_pp_Runner.Services;
using Firefly_iii_pp_Runner.Settings;
using Firefly_pp_Runner.KeyValueStore.Services;
using Firefly_pp_Runner.Persistence;
using Firefly_pp_Runner.Services;
using Firefly_pp_Runner.Settings;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.AutoReconcile;
using FireflyIIIppRunner.Abstractions.KeyValueStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Net.Sockets;

namespace Firefly_iii_pp_Runner.Extensions
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
            services.AddSingleton<IKeyValueStoreServiceFactory, KeyValueStoreServiceFactory>();


            return services;
        }

        public static IServiceCollection AddFilePersistenceServices(this IServiceCollection services)
        {
            services.AddSingleton<IPersistenceService, FilePersistenceService>();
            return services;
        }
        public static IServiceCollection AddMemoryPersistenceServices(this IServiceCollection services)
        {
            services.AddSingleton<IPersistenceService, MemoryPersistenceService>();
            return services;
        }
    }
}
