using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Services;

namespace FileService.Commands
{
    public class AcquireFileLockCommandHandler : ICommandHandler<AcquireFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly FileDbContext dbContext;

        public AcquireFileLockCommandHandler(IFileLockingService fileLockingService, FileDbContext dbContext)
        {
            this.fileLockingService = fileLockingService;
            this.dbContext = dbContext;
        }

        public void Handle(AcquireFileLockCommand command)
        {
            var file = dbContext.Files.FirstOrDefault(f => f.Id == command.FileId);

            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");

            fileLockingService.Lock(file);
        }
    }
}