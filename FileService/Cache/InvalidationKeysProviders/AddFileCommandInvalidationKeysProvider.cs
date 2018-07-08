using System.Collections.Generic;
using FileService.Commands;
using FileService.Queries;

namespace FileService.Cache.InvalidationKeysProviders
{
    internal class AddFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<AddFileCommand>
    {
        public IEnumerable<object> GetCacheKeysToInvalidate(AddFileCommand command)
        {
            return new[] { new FindFilesByUserQuery() {UserId = command.Owner.Id} };
        }
    }
}