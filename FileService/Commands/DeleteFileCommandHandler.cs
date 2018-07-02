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
            fileStorage.DeleteFile(command.Owner, command.FileId);
            fileDb.SaveChanges();
        }
    }
}