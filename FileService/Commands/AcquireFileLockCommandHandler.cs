using EventBus;
using FileService.Database;
using FileService.Dto;
using FileService.Events;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    public class AcquireFileLockCommandHandler : ICommandHandler<AcquireFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly IRepository<File> fileRepository;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;

        public AcquireFileLockCommandHandler(
            IFileLockingService fileLockingService, 
            IRepository<File> fileRepository, 
            IEventBusWrapper eventBus,
            IPostCommitRegistrator registrator)
        {
            this.fileLockingService = fileLockingService;
            this.eventBus = eventBus;
            this.registrator = registrator;
            this.fileRepository = fileRepository;
        }

        public void Handle(AcquireFileLockCommand command)
        {
            File file = fileRepository.GetById(command.FileId).EnsureFound(command.FileId);

            fileLockingService.Lock(file);
            UserDto lockOwner = fileLockingService.GetLockOwner(file);

            registrator.Committed += () =>
            {
                eventBus.Publish<FileLockChangedEvent, FileLockChangedMessage>(
                    new FileLockChangedMessage(command.FileId, FileLockDto.ForUser(lockOwner)));
            };
        }
    }
}