using FileService.Database;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    class DeleteFileCommandHandler: ICommandHandler<DeleteFileCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly FileDbContext fileDb;

        public DeleteFileCommandHandler(IFileStorage fileStorage, FileDbContext fileDb)
        {
            this.fileStorage = fileStorage;
            this.fileDb = fileDb;
        }

        public void Handle(DeleteFileCommand command)
        {
            fileDb.Files.Remove(new File() {Id = command.FileId});
            var owner = new User() {Id = command.OwnerUserId, UserName = command.OwnerUserId};
            fileStorage.DeleteFile(owner, command.FileId);
            fileDb.SaveChanges();
        }
    }
}