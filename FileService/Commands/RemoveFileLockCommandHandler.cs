using System.Linq;
using EventBus;
using FileService.Dto;
using FileService.Events;
using FileService.Exceptions;
using FileService.Infrastructure;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    public class RemoveFileLockCommandHandler : ICommandHandler<RemoveFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly IFileRepository fileRepository;
        private readonly ICurrentUser currentUser;
        private readonly IPostCommitEventPublisher eventBus;
        
        public RemoveFileLockCommandHandler(
            IFileLockingService fileLockingService, 
            IFileRepository fileRepository, 
            ICurrentUser currentUser, 
            IPostCommitEventPublisher eventBus)
        {
            this.fileLockingService = fileLockingService;
            this.fileRepository = fileRepository;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
        }

        public void Handle(RemoveFileLockCommand command)
        {
            File file = fileRepository.GetById(command.FileId);

            UserDto lockOwner = fileLockingService.GetRequiredLockOwner(file);
                
            if (lockOwner.Id != currentUser.Id)
                throw new PermissionException("The current user is not the lock owner");
            
            fileLockingService.Unlock(file, currentUser.ToDomainUser());
            
            UserDto newLockOwner = fileLockingService.GetLockOwner(file);

            eventBus.PublishAfterCommit<FileLockChangedEvent, FileLockChangedMessage>(
                new FileLockChangedMessage(command.FileId, FileLockDto.ForUser(newLockOwner)));
        }
    }
}