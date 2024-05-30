using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haondt.Web.Extensions;
using FireflyIIIpp.Web.Views;
using FireflyIIIpp.Web.Views.NodeRed;

namespace FireflyIIIpp.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFireflyIIIPPWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterPage("node-red", "~/Views/NodeRed/NodeRed.cshtml", data => new NodeRedModel());
            return services;
        }
    }
}
