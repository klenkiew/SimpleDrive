using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    internal class DeleteFileCommandHandler: ICommandHandler<DeleteFileCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly ICurrentUser currentUser;
        private readonly FileDbContext fileDb;

        public DeleteFileCommandHandler(IFileStorage fileStorage, ICurrentUser currentUser, FileDbContext fileDb)
        {
            this.fileStorage = fileStorage;
            this.currentUser = currentUser;
            this.fileDb = fileDb;
        }

        public void Handle(DeleteFileCommand command)
        {
            var file = fileDb.Files
                .Where(f => f.Id == command.FileId)
                .Include(f => f.SharedWith).ThenInclude(sh => sh.User)
                .Include(f => f.Owner)
                .FirstOrDefault();
            
            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");
            
            if (currentUser.Id != file.OwnerId)
                throw new PermissionException($"The user doesn't have a permission to delete the file with id {command.FileId}");
            
            fileDb.Files.Remove(file);
            fileStorage.DeleteFile(file);
            fileDb.SaveChanges();
        }
    }
}