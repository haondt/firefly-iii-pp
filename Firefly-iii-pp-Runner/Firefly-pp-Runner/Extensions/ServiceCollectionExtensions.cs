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
            var socketExceptionPolicy = Policy<HttpResponseMessage>
                .HandleInner<SocketException>()  // firefly-iii sometimes gives socket errors
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (e, t, i, c) =>
                {
                    logger.LogInformation("Retrying http call (retry attempt: {attempt}) due to socket exception", i);
                });
            var timeoutRejectionPolicy = Policy<HttpResponseMessage>
                .Handle<TimeoutRejectedException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(2), (e, t, i, c) =>
                {
                    logger.LogInformation("Retrying http call (retry attempt: {attempt}) due to timeout", i);
                });
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(30);
            return Policy.WrapAsync(socketExceptionPolicy, timeoutRejectionPolicy, timeoutPolicy);
        }
    }
}
