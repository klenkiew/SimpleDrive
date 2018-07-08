using System.Collections.Generic;
using FileService.Commands;
using FileService.Queries;
using FileService.Services;

namespace FileService.Cache.InvalidationKeysProviders
{
    internal class AddFileCommandInvalidationKeysProvider: IInvalidationKeysProvider<AddFileCommand>
    {
        private readonly ICurrentUser currentUser;

        public AddFileCommandInvalidationKeysProvider(ICurrentUser currentUser)
        {
            this.currentUser = currentUser;
        }

        public IEnumerable<object> GetCacheKeysToInvalidate(AddFileCommand command)
        {
            // TODO it only works because a file can be deleted only by the owner
            return new[] { new FindFilesByUserQuery() {UserId = currentUser.Id} };
        }
    }
}