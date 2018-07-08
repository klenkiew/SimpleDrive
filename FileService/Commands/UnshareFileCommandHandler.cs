using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    internal class UnshareFileCommandHandler : ICommandHandler<UnshareFileCommand>
    {
        private readonly ICurrentUser currentUser;
        private readonly FileDbContext fileDb;

        public UnshareFileCommandHandler(ICurrentUser currentUser, FileDbContext fileDb)
        {
            this.currentUser = currentUser;
            this.fileDb = fileDb;
        }

        public void Handle(UnshareFileCommand command)
        {
            var file = fileDb.Files
                .Where(f => f.Id == command.FileId)
                .Include(f => f.SharedWith)
                .ThenInclude(sh => sh.User)
                .FirstOrDefault();
            
            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");
            
            if (!HasPermissionToUnshare(command, file))
                throw new PermissionException($"The user doesn't have a permission to unshare the file with id {command.FileId}");
            
            foreach (var fileShare in file.SharedWith)
            {
                if (fileShare.FileId == command.FileId && fileShare.UserId == command.UserId)
                {
                    file.SharedWith.Remove(fileShare);
                    break;
                }
            }

            fileDb.SaveChanges();
        }

        private bool HasPermissionToUnshare(UnshareFileCommand command, File file)
        {
            return currentUser.Id == file.OwnerId || command.UserId == currentUser.Id;
        }
    }
}