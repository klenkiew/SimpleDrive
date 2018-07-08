using System;
using System.Linq;
using FileService.Database;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class AddFileCommandHandler : ICommandHandler<AddFileCommand>
    {
        private readonly FileDbContext fileDb;
        private readonly IFileStorage fileStorage;
        private readonly ICurrentUser currentUser;

        public AddFileCommandHandler(FileDbContext fileDb, IFileStorage fileStorage, ICurrentUser currentUser)
        {
            this.fileDb = fileDb;
            this.fileStorage = fileStorage;
            this.currentUser = currentUser;
        }

        public void Handle(AddFileCommand command)
        {
            var owner = fileDb.Users.FirstOrDefault(u => u.Id == currentUser.Id);

            if (owner == null)
            {
                owner = new User() {Id = currentUser.Id, Username =currentUser.Username};
                fileDb.Users.Add(owner);
            }

            var file = new File()
            {
                FileName = command.FileName,
                Description = command.Description,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                PhysicalPath = "N/A",
                Size = command.Content.Length,
                OwnerId = owner.Id,
                OwnerName = owner.Username,
                Owner = owner
            };
            fileDb.Files.Add(file);
            fileDb.SaveChanges();

            fileStorage.SaveFile(file, command.Content);
        }
    }
}