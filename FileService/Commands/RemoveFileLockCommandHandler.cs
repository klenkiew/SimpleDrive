using System.Linq;
using EventBus;
using FileService.Database;
using FileService.Dto;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    public class RemoveFileLockCommandHandler : ICommandHandler<RemoveFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly IRepository<File> fileRepository;
        private readonly ICurrentUser currentUser;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;
        
        public RemoveFileLockCommandHandler(
            IFileLockingService fileLockingService, 
            IRepository<File> fileRepository, 
            ICurrentUser currentUser, 
            IEventBusWrapper eventBus, 
            IPostCommitRegistrator registrator)
        {
            this.fileLockingService = fileLockingService;
            this.fileRepository = fileRepository;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
            this.registrator = registrator;
        }

        public void Handle(RemoveFileLockCommand command)
        {
            File file = fileRepository.GetById(command.FileId);

            UserDto lockOwner = fileLockingService.GetRequiredLockOwner(file);
                
            if (lockOwner.Id != currentUser.Id)
                throw new PermissionException("The current user is not the lock owner");
            
            fileLockingService.Unlock(file);
            
            UserDto newLockOwner = fileLockingService.GetLockOwner(file);

            registrator.Committed += () =>
            {
                eventBus.Publish<FileLockChangedEvent, FileLockChangedMessage>(
                    new FileLockChangedMessage(command.FileId, FileLockDto.ForUser(newLockOwner)));
            };
        }
    }
}