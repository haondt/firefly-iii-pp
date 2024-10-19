using Firefly_pp_Runner.Lookup.Services;
using Haondt.Persistence.Postgresql.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly_pp_Runner.Lookup.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLookupServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PostgresqlStorageSettings>(configuration.GetSection(nameof(PostgresqlStorageSettings)));
            services.AddSingleton<ILookupStorage, PostgresqlLookupStorage>();

            services.Configure<LookupSettings>(configuration.GetSection(nameof(LookupSettings)));
            services.AddSingleton<ILookupStoreProvider, LookupStoreProvider>();

            return services;
        }

    }
}
