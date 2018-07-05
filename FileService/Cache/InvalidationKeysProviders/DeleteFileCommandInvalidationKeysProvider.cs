using System.Collections.Generic;
using FileService.Commands;
using FileService.Queries;

namespace FileService.Cache.InvalidationKeysProviders
{
    internal class DeleteFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<DeleteFileCommand>
    {
        public IEnumerable<object> GetCacheKeysToInvalidate(DeleteFileCommand command)
        {
            return new[] { new FindFilesByOwnerQuery() {OwnerId = command.Owner.Id} };
        }
    }
}