using System.Collections.Generic;
using FileService.Queries;
using FileService.Services;

namespace FileService.Commands.InvalidationKeysProviders
{
    internal class EditFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<EditFileCommand>
    {
        private readonly ICurrentUser currentUser;

        public EditFileCommandInvalidationKeysProvider(ICurrentUser currentUser)
        {
            this.currentUser = currentUser;
        }

        public IEnumerable<object> GetCacheKeysToInvalidate(EditFileCommand command)
        {
            // TODO it only works because a file can be deleted only by the owner
            // TODO invalidate the cache entires of other users who have access to this file 
            return new[] { new FindFilesByUserQuery(currentUser.Id) };
        }
    }
}