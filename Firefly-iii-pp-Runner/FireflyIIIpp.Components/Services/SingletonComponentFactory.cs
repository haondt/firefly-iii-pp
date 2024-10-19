﻿using Haondt.Web.Core.Components;

namespace FireflyIIIpp.Components.Services
{
    public class SingletonComponentFactory(IServiceProvider serviceProvider) : ISingletonComponentFactory
    {
        public IComponentFactory CreateComponentFactory()
        {
            using var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IComponentFactory>();
        }
    }
}
