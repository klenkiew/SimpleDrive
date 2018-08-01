using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class EditFileCommandHandler : ICommandHandler<EditFileCommand>
    {
        private readonly IFileRepository fileRepository;
        private readonly ICurrentUser currentUser;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;
        
        public EditFileCommandHandler(
            IFileRepository fileRepository, 
            ICurrentUser currentUser, 
            IEventBusWrapper eventBus, 
            IPostCommitRegistrator registrator)
        {
            this.fileRepository = fileRepository;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
            this.registrator = registrator;
        }

        public void Handle(EditFileCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            if (!file.IsOwnedBy(currentUser.ToDomainUser()))
                throw new PermissionException($"The user doesn't have a permission to edit the file with id {command.FileId}");

            file.Edit(command.FileName, command.Description);
            fileRepository.Update(file);
            
            registrator.Committed += () => eventBus.Publish<FileEditedEvent, File>(file);
        }
    }
}