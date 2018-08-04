using FileService.Dto;
using FileService.Events;
using FileService.Exceptions;
using FileService.Infrastructure;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    public class AcquireFileLockCommandHandler : ICommandHandler<AcquireFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly ICurrentUserSource currentUserSource;
        private readonly IFileRepository fileRepository;
        private readonly IPostCommitEventPublisher eventBus;

        public AcquireFileLockCommandHandler(
            IFileLockingService fileLockingService, 
            IFileRepository fileRepository, 
            IPostCommitEventPublisher eventBus, 
            ICurrentUserSource currentUserSource)
        {
            this.fileLockingService = fileLockingService;
            this.eventBus = eventBus;
            this.currentUserSource = currentUserSource;
            this.fileRepository = fileRepository;
        }

        public void Handle(AcquireFileLockCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);

            User currentUser = currentUserSource.GetCurrentUser();
            
            if (!file.CanBeModifiedBy(currentUser))
                throw new PermissionException($"The user doesn't have a permission to lock the file with id {command.FileId}");
            
            fileLockingService.Lock(file, currentUser);
            UserDto lockOwner = fileLockingService.GetLockOwner(file);
         
            eventBus.PublishAfterCommit<FileLockChangedEvent, FileLockChangedMessage>(
                    new FileLockChangedMessage(command.FileId, FileLockDto.ForUser(lockOwner)));
        }
    }
}