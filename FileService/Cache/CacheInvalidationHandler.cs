using System;
using System.Collections.Generic;
using FileService.Commands;

namespace FileService.Cache
{
    internal class CacheInvalidationHandler<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decorated;
        private readonly IUniversalCache cache;
        private readonly Func<TCommand, object> invalidationKeysProvider;

        public CacheInvalidationHandler(
            ICommandHandler<TCommand> decorated, 
            IUniversalCache cache, 
            Func<TCommand, IEnumerable<object>> invalidationKeysProvider)
        {
            this.decorated = decorated;
            this.cache = cache;
            this.invalidationKeysProvider = invalidationKeysProvider;
        }

        public void Handle(TCommand command)
        {
            decorated.Handle(command);
            cache.Remove(invalidationKeysProvider(command));
        }
    }
}