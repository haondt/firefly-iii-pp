using Firefly_iii_pp.EventHandlers;
using Firefly_iii_pp.EventHandlers.Lookup;
using Firefly_iii_pp.EventHandlers.NodeRed;
using Firefly_iii_pp.Middlewares;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;

namespace Firefly_iii_pp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFireflyIIIppServices(this IServiceCollection services, IConfiguration configuration)
        {
            // event handlers
            services.AddScoped<IEventHandler, SingleEventHandlerRegistry>();

            services.AddScoped<ISingleEventHandler, SendToNodeRedEventHandler>();

            services.AddScoped<ISingleEventHandler, NavigateEventHandler>();

            services.AddScoped<ISingleEventHandler, GetValueEventHandler>();
            services.AddScoped<ISingleEventHandler, PrettifyValueEventHandler>();
            services.AddScoped<ISingleEventHandler, UpsertValueEventHandler>();
            services.AddScoped<ISingleEventHandler, DeleteValueEventHandler>();
            services.AddScoped<ISingleEventHandler, PrimaryKeyAutocompleteEventHandler>();
            services.AddScoped<ISingleEventHandler, ForeignKeyAutocompleteEventHandler>();
            services.AddScoped<ISingleEventHandler, MapForeignKeyEventHandler>();
            services.AddScoped<ISingleEventHandler, GetForeignKeysEventHandler>();
            services.AddScoped<ISingleEventHandler, DeleteForeignKeyEventHandler>();

            // middleware
            services.AddSingleton<IExceptionActionResultFactory, ToastExceptionActionResultFactory>();

            return services;
        }

        public static IServiceCollection AddFireflyIIIppHeadEntries(this IServiceCollection services)
        {
            services.AddScoped<IHeadEntryDescriptor>(sp => new ScriptDescriptor
            {
                Uri = "https://kit.fontawesome.com/afd44816da.js",
                CrossOrigin = "anonymous"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new MetaDescriptor
            {
                Name = "htmx-config",
                Content = @"{
                    ""responseHandling"": [
                        { ""code"": ""204"", ""swap"": false },
                        { ""code"": "".*"", ""swap"": true }
                    ]
                }",
            });
            return services;
        }

    }
}
