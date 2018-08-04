using EventBus;
using FileService.Events;
using FileService.Exceptions;
using FileService.Infrastructure;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class UnshareFileCommandHandler : ICommandHandler<UnshareFileCommand>
    {
        private readonly ICurrentUser currentUser;
        private readonly IFileRepository fileRepository;
        private readonly IPostCommitEventPublisher eventBus;

        public UnshareFileCommandHandler(
            ICurrentUser currentUser, 
            IFileRepository fileRepository, 
            IPostCommitEventPublisher eventBus)
        {
            this.currentUser = currentUser;
            this.fileRepository = fileRepository;
            this.eventBus = eventBus;
        }

        public void Handle(UnshareFileCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            if (!HasPermissionToUnshare(command, file))
                throw new PermissionException($"The user doesn't have a permission to unshare the file with id {command.FileId}");
            
            file.Unshare(new User(command.UserId, "N/A"));

            eventBus.PublishAfterCommit<FileSharedEvent, FileSharesChangedMessage>(
                new FileSharesChangedMessage(file, command.UserId));
        }

        private bool HasPermissionToUnshare(UnshareFileCommand command, File file)
        {
            return file.IsOwnedBy(currentUser.ToDomainUser()) || command.UserId == currentUser.Id;
        }
    }
}