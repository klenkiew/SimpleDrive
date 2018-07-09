using System.Collections.Generic;
using FileService.Queries;

namespace FileService.Commands.InvalidationKeysProviders
{
    internal class ShareFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<ShareFileCommand>
    {
        public IEnumerable<object> GetCacheKeysToInvalidate(ShareFileCommand command)
        {
            return new object[]
            {
                new FindFilesByUserQuery() {UserId = command.ShareWithUserId},
                new FindUsersBySharedFileQuery() {FileId = command.FileId}
            };
        }
    }
}