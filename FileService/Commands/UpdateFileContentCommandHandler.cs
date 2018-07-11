using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    public class UpdateFileContentCommandHandler : ICommandHandler<UpdateFileContentCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly FileDbContext dbContext;
        private readonly ICurrentUser currentUser;

        public UpdateFileContentCommandHandler(IFileStorage fileStorage, FileDbContext dbContext, ICurrentUser currentUser)
        {
            this.fileStorage = fileStorage;
            this.dbContext = dbContext;
            this.currentUser = currentUser;
        }

        public void Handle(UpdateFileContentCommand command)
        {
            var file = dbContext.Files
                .Where(f => f.Id == command.FileId)
                .Include(f => f.SharedWith).ThenInclude(sh => sh.User)
                .Include(f => f.Owner)
                .FirstOrDefault();
            
            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");
            
            if (!HasPermissionToModifyContent(file))
                throw new PermissionException($"The user doesn't have a permission to update the content of the file with id {command.FileId}");

            fileStorage.UpdateFile(file, command.Content);
        }
        
        private bool HasPermissionToModifyContent(File file)
        {
            return currentUser.Id == file.OwnerId || file.SharedWith.Any(sh => sh.UserId == currentUser.Id);
        }
    }
}