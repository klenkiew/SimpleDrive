using FileService.Queries;

namespace FileService.Cache
{
    internal class CachedQuery<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> decorated;
        private readonly IUniversalCache cache;
        
        public CachedQuery(IQueryHandler<TQuery, TResult> decorated, IUniversalCache cache)
        {
            this.decorated = decorated;
            this.cache = cache;
        }

        public TResult Handle(TQuery query)
        {
            return cache.ComputeIfAbsent(query, () => decorated.Handle(query));
        }
    }
}