using FileService.Cache;
using FileService.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FileService.Queries
{
    public static class QueryHandlerRegistration
    {
        public static IServiceCollection RegisterQueryHandler<TQueryHandler, TQuery, TResult>(
            this IServiceCollection services) 
            where TQuery : IQuery<TResult>
            where TQueryHandler : class, IQueryHandler<TQuery, TResult>
        {
            services.AddTransient<TQueryHandler>();

            services.AddTransient<IQueryHandler<TQuery, TResult>>(sc =>
                new CachedQuery<TQuery, TResult>(
                    new LoggedQuery<TQuery, TResult>(
                        sc.GetService<TQueryHandler>(), 
                        sc.GetService<ILogger<IQueryHandler<TQuery, TResult>>>(), 
                        sc.GetService<ISerializer>()), 
                    sc.GetService<IUniversalCache>()));

            return services;
        }
    }
}