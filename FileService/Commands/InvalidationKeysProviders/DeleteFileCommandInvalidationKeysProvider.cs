using System.Collections.Generic;
using FileService.Queries;
using FileService.Services;

namespace FileService.Commands.InvalidationKeysProviders
{
    internal class DeleteFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<DeleteFileCommand>
    {
        private readonly ICurrentUser currentUser;

        public DeleteFileCommandInvalidationKeysProvider(ICurrentUser currentUser)
        {
            this.currentUser = currentUser;
        }

        public IEnumerable<object> GetCacheKeysToInvalidate(DeleteFileCommand command)
        {
            return new object[]
            {
                // TODO it only works because a file can be deleted only by the owner
                new FindFilesByUserQuery(currentUser.Id), 
                new FindFileByIdQuery(command.FileId),
                new FindUsersBySharedFileQuery(command.FileId)
            };
        }
    }
}