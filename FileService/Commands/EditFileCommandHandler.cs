using System;
using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Infrastructure;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    public class EditFileCommandHandler : ICommandHandler<EditFileCommand>
    {
        private readonly IFileRepository fileRepository;
        private readonly ICurrentUser currentUser;
        private readonly IPostCommitEventPublisher eventBus;
        
        public EditFileCommandHandler(
            IFileRepository fileRepository, 
            ICurrentUser currentUser, 
            IPostCommitEventPublisher eventBus)
        {
            this.fileRepository = fileRepository;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
        }

        public void Handle(EditFileCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            if (!file.IsOwnedBy(currentUser.ToDomainUser()))
                throw new PermissionException($"The user doesn't have a permission to edit the file with id {command.FileId}");

            file.Edit(command.FileName, command.Description, DateTime.UtcNow);
            fileRepository.Update(file);
            
            eventBus.PublishAfterCommit<FileEditedEvent, File>(file);
        }
    }
}