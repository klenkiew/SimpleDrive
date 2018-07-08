using System.Collections.Generic;
using FileService.Commands;
using FileService.Queries;

namespace FileService.Cache.InvalidationKeysProviders
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