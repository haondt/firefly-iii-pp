using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.FireflyIII.Services;
using FireflyIIIpp.FireflyIII.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.FireflyIII.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFireflyIIIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FireflyIIISettings>(configuration.GetSection(nameof(FireflyIIISettings)));

            services.AddSingleton<IFireflyIIIService, FireflyIIIService>();

            services.AddHttpClient<FireflyIIIService>()
                .AddPolicyHandler(GetFireflyIIIPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetFireflyIIIPolicy()
        {
            var logger = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            }).CreateLogger<Policy>();
            var socketExceptionPolicy = Policy<HttpResponseMessage>
                .HandleInner<SocketException>()  // firefly-iii sometimes gives socket errors
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (e, t, i, c) =>
                {
                    logger.LogInformation("Retrying http call (retry attempt: {attempt}) due to socket exception", i);
                });
            var timeoutRejectionPolicy = Policy<HttpResponseMessage>
                .Handle<TimeoutRejectedException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2), (e, t, i, c) =>
                {
                    logger.LogInformation("Retrying http call (retry attempt: {attempt}) due to timeout", i);
                });
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);
            return Policy.WrapAsync(socketExceptionPolicy, timeoutRejectionPolicy, timeoutPolicy);
        }
    }
}
