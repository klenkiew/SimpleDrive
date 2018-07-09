using System.Collections.Generic;
using FileService.Queries;

namespace FileService.Commands.InvalidationKeysProviders
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