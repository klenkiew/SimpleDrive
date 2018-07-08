using FileService.Database;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class DeleteFileCommandHandler: ICommandHandler<DeleteFileCommand>
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
            // TODO throw if the file doesn't exist?
            // TODO check if the current user is the owner of the file
            fileDb.Files.Remove(new File() {Id = command.FileId});
            fileStorage.DeleteFile(command.Owner, command.FileId);
            fileDb.SaveChanges();
        }
    }
}