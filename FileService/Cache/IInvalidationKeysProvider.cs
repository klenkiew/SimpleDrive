using System.Collections.Generic;

namespace FileService.Cache
{
    interface IInvalidationKeysProvider<TCommand>
    {
        IEnumerable<object> GetCacheKeysToInvalidate(TCommand command);
    }
}