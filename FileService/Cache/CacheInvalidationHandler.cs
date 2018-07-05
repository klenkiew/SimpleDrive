using System;
using FileService.Commands;

namespace FileService.Cache
{
    internal class CacheInvalidationHandler<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decorated;
        private readonly IUniversalCache cache;
        private readonly IInvalidationKeysProvider<TCommand> invalidationKeysProvider;

        public CacheInvalidationHandler(
            ICommandHandler<TCommand> decorated, 
            IUniversalCache cache, 
            IInvalidationKeysProvider<TCommand> invalidationKeysProvider)
        {
            this.decorated = decorated;
            this.cache = cache;
            this.invalidationKeysProvider = invalidationKeysProvider;
        }

        public void Handle(TCommand command)
        {
            decorated.Handle(command);
            var cacheKeysToInvalidate = invalidationKeysProvider.GetCacheKeysToInvalidate(command);
            foreach (var key in cacheKeysToInvalidate)
                cache.Remove(key);
        }
    }
}