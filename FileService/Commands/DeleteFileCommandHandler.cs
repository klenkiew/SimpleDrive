using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class DeleteFileCommandHandler: ICommandHandler<DeleteFileCommand>
    {
        private readonly IFileStorage fileStorage;
        private readonly ICurrentUser currentUser;
        private readonly IRepository<File> fileRepository;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;
        
        public DeleteFileCommandHandler(
            IFileStorage fileStorage, 
            ICurrentUser currentUser, 
            IRepository<File> fileRepository,
            IEventBusWrapper eventBus, 
            IPostCommitRegistrator registrator)
        {
            this.fileStorage = fileStorage;
            this.currentUser = currentUser;
            this.fileRepository = fileRepository;
            this.eventBus = eventBus;
            this.registrator = registrator;
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
            
            registrator.Committed += () => eventBus.Publish<FileDeletedEvent, File>(file);
        }
    }
}