using System.Collections.Generic;

namespace FileService.Commands.InvalidationKeysProviders
{
    interface IInvalidationKeysProvider<TCommand>
    {
        IEnumerable<object> GetCacheKeysToInvalidate(TCommand command);
    }
}