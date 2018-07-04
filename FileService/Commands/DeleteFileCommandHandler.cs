using FileService.Cache;
using FileService.Database;
using FileService.Model;
using FileService.Queries;
using FileService.Services;

namespace FileService.Commands
{
    internal class DeleteFileCommandHandler: ICommandHandler<DeleteFileCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly FileDbContext fileDb;
        private readonly IUniversalCache cache;

        public DeleteFileCommandHandler(IFileStorage fileStorage, FileDbContext fileDb, IUniversalCache cache)
        {
            this.fileStorage = fileStorage;
            this.fileDb = fileDb;
            this.cache = cache;
        }

        public void Handle(DeleteFileCommand command)
        {
            fileDb.Files.Remove(new File() {Id = command.FileId});
            fileStorage.DeleteFile(command.Owner, command.FileId);
            fileDb.SaveChanges();
            
            // invalidate the cache
            var query = new FindFilesByOwnerQuery()
            {
                OwnerId = command.Owner.Id
            };
            cache.Remove(query);
        }
    }
}