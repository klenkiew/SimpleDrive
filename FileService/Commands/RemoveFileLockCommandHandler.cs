using System.Linq;
using FileService.Database;
using FileService.Exceptions;
using FileService.Services;

namespace FileService.Commands
{
    public class RemoveFileLockCommandHandler : ICommandHandler<RemoveFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly FileDbContext dbContext;
        private readonly ICurrentUser currentUser;

        public RemoveFileLockCommandHandler(
            IFileLockingService fileLockingService, 
            FileDbContext dbContext, 
            ICurrentUser currentUser)
        {
            this.fileLockingService = fileLockingService;
            this.dbContext = dbContext;
            this.currentUser = currentUser;
        }

        public void Handle(RemoveFileLockCommand command)
        {
            var file = dbContext.Files.FirstOrDefault(f => f.Id == command.FileId);

            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");

            var lockOwner = fileLockingService.GetLockOwner(file);
            
            if (lockOwner == null)
                throw new NotFoundException($"The file with id {command.FileId} is not locked.");
                
            if (lockOwner.Id != currentUser.Id)
                throw new PermissionException("The current user is not the lock owner");
            
            fileLockingService.Unlock(file);
        }
    }
}