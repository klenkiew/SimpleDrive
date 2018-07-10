using System;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace FileService.Infrastructure.ScopedServices
{
    public class ServiceScope<TService> : IServiceScope<TService> where TService : class, IDisposable
    {
        private readonly Scope scope;

        public ServiceScope(Container container)
        {
            scope = AsyncScopedLifestyle.BeginScope(container);
        }

        public TService GetService()
        {
            return scope.GetInstance<TService>();
        }
        
        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}