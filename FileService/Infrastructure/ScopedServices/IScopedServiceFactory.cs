using System;

namespace FileService.Infrastructure.ScopedServices
{
    public interface IScopedServiceFactory<out TService> where TService : class, IDisposable
    {
        IServiceScope<TService> CreateScope();
    }
}