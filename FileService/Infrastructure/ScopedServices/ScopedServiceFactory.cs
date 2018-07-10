using System;
using SimpleInjector;

namespace FileService.Infrastructure.ScopedServices
{
    public class ScopedServiceFactory<TService> : IScopedServiceFactory<TService> where TService : class, IDisposable
    {
        private readonly Container container;

        public ScopedServiceFactory(Container container)
        {
            this.container = container;
        }

        public IServiceScope<TService> CreateScope()
        {
            return new ServiceScope<TService>(container);
        }
    }
}