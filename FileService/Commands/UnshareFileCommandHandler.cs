using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class UnshareFileCommandHandler : ICommandHandler<UnshareFileCommand>
    {
        private readonly ICurrentUser currentUser;
        private readonly IRepository<File> fileRepository;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;

        public UnshareFileCommandHandler(
            ICurrentUser currentUser, 
            IRepository<File> fileRepository, 
            IEventBusWrapper eventBus, 
            IPostCommitRegistrator registrator)
        {
            this.currentUser = currentUser;
            this.fileRepository = fileRepository;
            this.eventBus = eventBus;
            this.registrator = registrator;
        }

        public void Handle(UnshareFileCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            if (!HasPermissionToUnshare(command, file))
                throw new PermissionException($"The user doesn't have a permission to unshare the file with id {command.FileId}");
            
            file.Unshare(new User(command.UserId, "N/A"));
//            fileDb.SaveChanges();

            registrator.Committed += () =>
            {
                eventBus.Publish<FileSharedEvent, FileSharesChangedMessage>(
                    new FileSharesChangedMessage(file, command.UserId));
            };
        }

        private bool HasPermissionToUnshare(UnshareFileCommand command, File file)
        {
            return file.IsOwnedBy(currentUser.ToDomainUser()) || command.UserId == currentUser.Id;
        }
    }
}