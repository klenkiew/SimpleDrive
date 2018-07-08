using System.Collections.Generic;
using FileService.Commands;
using FileService.Queries;

namespace FileService.Cache.InvalidationKeysProviders
{
    internal class UnshareFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<UnshareFileCommand>
    {
        public IEnumerable<object> GetCacheKeysToInvalidate(UnshareFileCommand command)
        {
            return new object[]
            {
                new FindFilesByUserQuery() {UserId = command.UserId},
                new FindUsersBySharedFileQuery() {FileId = command.FileId}
            };
        }
    }
}