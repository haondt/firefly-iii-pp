using Firefly_iii_pp_Runner.API.Services;
using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Firefly_iii_pp_Runner.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFireflyIIIPPRunnerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FireflyIIISettings>(configuration.GetSection(nameof(FireflyIIISettings)));
            services.Configure<NodeRedSettings>(configuration.GetSection(nameof(NodeRedSettings)));

            services.AddSingleton<FireflyIIIService>();
            services.AddSingleton<NodeRedService>();
            services.AddSingleton<JobManager>();

            services.AddHttpClient<FireflyIIIService>()
                .SetHandlerLifetime(TimeSpan.FromSeconds(5))
                .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
