using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class ShareFileCommandHandler : ICommandHandler<ShareFileCommand>
    {
        private readonly ICurrentUser currentUser;
        private readonly IFileRepository fileRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;

        public ShareFileCommandHandler(
            ICurrentUser currentUser, 
            IFileRepository fileRepository, 
            IUserRepository userRepository, 
            IEventBusWrapper eventBus, 
            IPostCommitRegistrator registrator)
        {
            this.currentUser = currentUser;
            this.fileRepository = fileRepository;
            this.eventBus = eventBus;
            this.registrator = registrator;
            this.userRepository = userRepository;
        }

        public void Handle(ShareFileCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);
            
            
            if (!file.IsOwnedBy(currentUser.ToDomainUser()))
                throw new PermissionException($"The user doesn't have a permission to share the file with id {command.FileId}");

            User shareWith = userRepository.GetById(command.ShareWithUserId);
            
            if (shareWith == null)
                throw new NotFoundException($"A user with id {command.ShareWithUserId} doesn't exist in the database.");
            

            file.ShareWith(shareWith);

            registrator.Committed += () =>
            {
                eventBus.Publish<FileSharedEvent, FileSharesChangedMessage>(
                    new FileSharesChangedMessage(file, command.ShareWithUserId));
            };
        }
    }
}