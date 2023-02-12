using FireflyIIIpp.FireflyIII.Abstractions;
using FireflyIIIpp.Mock.API.Fakes;
using FireflyIIIpp.Mock.API.Settings;
using FireflyIIIpp.NodeRed.Abstractions;

namespace FireflyIIIpp.Mock.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMockFireflyIIIPPServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INodeRedService, FakeNodeRedService>();
            services.AddSingleton<IFireflyIIIService, FakeFireflyIIIService>();

            services.Configure<FakeFireflyIIIServiceSettings>(configuration.GetSection(nameof(FakeFireflyIIIServiceSettings)));

            return services;
        }

    }
}
