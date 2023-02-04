using Firefly_iii_pp_Runner.API.Services;
using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Firefly_iii_pp_Runner.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFireflyIIIPPRunnerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FireflyIIISettings>(configuration.GetSection(nameof(FireflyIIISettings)));
            services.Configure<NodeRedSettings>(configuration.GetSection(nameof(NodeRedSettings)));
            services.Configure<ThunderClientEditorSettings>(configuration.GetSection(nameof(ThunderClientEditorSettings)));

            services.AddSingleton<FireflyIIIService>();
            services.AddSingleton<NodeRedService>();
            services.AddSingleton<JobManager>();
            services.AddSingleton<ThunderClientEditorService>();

            services.AddHttpClient<FireflyIIIService>()
                .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var logger = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            }).CreateLogger<Policy>();
            var transientErrorPolicy = HttpPolicyExtensions
                .HandleTransientHttpError() // firefly-iii sometimes gives socket errors
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2), (e, t, i, c) =>
                {
                    logger.LogInformation("Retrying http call (retry attempt: {attempt}) due to error: {error}", i, e.Exception.Message);
                });
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(2);
            return Policy.WrapAsync(transientErrorPolicy, timeoutPolicy);
        }

        public static IServiceCollection AddMongoServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
            services.AddSingleton<MongoService>();
            return services;
        }
    }
}
