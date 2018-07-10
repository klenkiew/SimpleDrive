using System;

namespace FileService.Infrastructure.ScopedServices
{
    public interface IServiceScope<out TService> : IDisposable where TService : class, IDisposable
    {
        TService GetService();
    }
}