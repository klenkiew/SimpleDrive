using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    internal class EditFileCommandHandler : ICommandHandler<EditFileCommand>
    {
        private readonly FileDbContext fileDb;
        private readonly ICurrentUser currentUser;

        public EditFileCommandHandler(FileDbContext fileDb,ICurrentUser currentUser)
        {
            this.fileDb = fileDb;
            this.currentUser = currentUser;
        }

        public void Handle(EditFileCommand command)
        {
            var file = fileDb.Files.Include(f => f.Owner).FirstOrDefault(f => f.Id == command.FileId);

            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");
            
            if (currentUser.Id != file.OwnerId)
                throw new PermissionException($"The user doesn't have a permission to edit the file with id {command.FileId}");

            file.FileName = command.FileName;
            file.Description = command.Description;
            fileDb.SaveChanges();
        }
    }
}