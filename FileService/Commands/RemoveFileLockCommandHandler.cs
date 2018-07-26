using System.Linq;
using EventBus;
using FileService.Database;
using FileService.Dto;
using FileService.Events;
using FileService.Exceptions;
using FileService.Services;

namespace FileService.Commands
{
    public class RemoveFileLockCommandHandler : ICommandHandler<RemoveFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly FileDbContext dbContext;
        private readonly ICurrentUser currentUser;
        private readonly IEventBusWrapper eventBus;

        public RemoveFileLockCommandHandler(
            IFileLockingService fileLockingService, 
            FileDbContext dbContext, 
            ICurrentUser currentUser, 
            IEventBusWrapper eventBus)
        {
            this.fileLockingService = fileLockingService;
            this.dbContext = dbContext;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
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
            
            var newLockOwner = fileLockingService.GetLockOwner(file);

            eventBus.Publish<FileLockChangedEvent, FileLockChangedMessage>(new FileLockChangedEvent(
                new FileLockChangedMessage(command.FileId, FileLockDto.ForUser(newLockOwner))));
        }
    }
}