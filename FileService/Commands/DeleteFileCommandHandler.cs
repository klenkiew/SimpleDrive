using EventBus;
using FileService.Events;
using FileService.Exceptions;
using FileService.Infrastructure;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    public class DeleteFileCommandHandler: ICommandHandler<DeleteFileCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly ICurrentUser currentUser;
        private readonly IFileRepository fileRepository;
        private readonly IPostCommitEventPublisher eventBus;

        public DeleteFileCommandHandler(
            IFileStorage fileStorage, 
            ICurrentUser currentUser, 
            IFileRepository fileRepository,
            IPostCommitEventPublisher eventBus)
        {
            this.fileStorage = fileStorage;
            this.currentUser = currentUser;
            this.fileRepository = fileRepository;
            this.eventBus = eventBus;
        }

        public void Handle(DeleteFileCommand command)
        {
            File file = fileRepository.GetById(command.FileId)
                .EnsureFound(command.FileId);
            
            if (!file.IsOwnedBy(currentUser.ToDomainUser()))
                throw new PermissionException($"The user doesn't have a permission to delete the file with id {command.FileId}");

            // ensure that the file is deleted from disk before deleting the entity 
            fileRepository.Delete(file);
            fileStorage.DeleteFile(file);
            
            eventBus.PublishAfterCommit<FileDeletedEvent, File>(file);
        }
    }
}