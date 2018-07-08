using System.Collections.Generic;
using FileService.Commands;
using FileService.Queries;

namespace FileService.Cache.InvalidationKeysProviders
{
    internal class DeleteFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<DeleteFileCommand>
    {
        public IEnumerable<object> GetCacheKeysToInvalidate(DeleteFileCommand command)
        {
            return new object[]
            {
                new FindFilesByUserQuery() {UserId = command.Owner.Id},
                new FindFileByIdQuery() {FileId = command.FileId},
                new FindUsersBySharedFileQuery() {FileId = command.FileId}
            };
        }
    }
}