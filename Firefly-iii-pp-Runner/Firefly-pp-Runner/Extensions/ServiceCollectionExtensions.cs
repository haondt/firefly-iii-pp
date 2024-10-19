using Firefly_iii_pp_Runner.Settings;
using Firefly_pp_Runner.Autoreconcile;
using Firefly_pp_Runner.Services;
using Firefly_pp_Runner.Settings;
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
            services.AddSingleton<IAutoReconcileService, AutoReconcileService>();


            return services;
        }

    }
}
