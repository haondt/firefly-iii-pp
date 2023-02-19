using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.Mock.API.Fakes;
using FireflyIIIpp.Mock.API.Settings;
using FireflyIIIpp.NodeRed.Abstractions;
using FireflyIIIppRunner.Abstractions;
using FireflyIIIppRunner.Abstractions.AutoReconcile;

namespace FireflyIIIpp.Mock.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMockFireflyIIIPPServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INodeRedService, FakeNodeRedService>();
            services.AddSingleton<IFireflyIIIService, FakeFireflyIIIService>();
            services.AddSingleton<IAutoReconcileService, FakeAutoReconcileService>();

            services.Configure<FakeFireflyIIIServiceSettings>(configuration.GetSection(nameof(FakeFireflyIIIServiceSettings)));

            return services;
        }

    }
}
