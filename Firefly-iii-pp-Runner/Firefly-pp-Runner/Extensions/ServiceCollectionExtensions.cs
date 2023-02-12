using Firefly_iii_pp_Runner.Services;
using Firefly_iii_pp_Runner.Settings;
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
            services.Configure<NodeRedSettings>(configuration.GetSection(nameof(NodeRedSettings)));
            services.Configure<ThunderClientEditorSettings>(configuration.GetSection(nameof(ThunderClientEditorSettings)));

            services.AddSingleton<NodeRedService>();
            services.AddSingleton<JobManager>();
            services.AddSingleton<ThunderClientEditorService>();

            services.AddHttpClient<NodeRedService>()
                .AddPolicyHandler(GetNodeRedPolicy());

            return services;
        }
        private static IAsyncPolicy<HttpResponseMessage> GetNodeRedPolicy()
        {
            var logger = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            }).CreateLogger<Policy>();
            var timeoutPolicy = Policy
                .TimeoutAsync<HttpResponseMessage>(1, async (ct, ts, t) =>
                {
                    logger.LogInformation("Timed out NodeRed request.");
                });
            return timeoutPolicy;
        }

    }
}
