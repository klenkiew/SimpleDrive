using System.Linq;
using EventBus;
using FileService.Database;
using FileService.Dto;
using FileService.Events;
using FileService.Exceptions;
using FileService.Services;

namespace FileService.Commands
{
    public class AcquireFileLockCommandHandler : ICommandHandler<AcquireFileLockCommand>
    {
        private readonly IFileLockingService fileLockingService;
        private readonly FileDbContext dbContext;
        private readonly IEventBusWrapper eventBus;

        public AcquireFileLockCommandHandler(IFileLockingService fileLockingService, FileDbContext dbContext, IEventBusWrapper eventBus)
        {
            this.fileLockingService = fileLockingService;
            this.dbContext = dbContext;
            this.eventBus = eventBus;
        }

        public void Handle(AcquireFileLockCommand command)
        {
            var file = dbContext.Files.FirstOrDefault(f => f.Id == command.FileId);

            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");

            fileLockingService.Lock(file);
            var lockOwner = fileLockingService.GetLockOwner(file);
            
            eventBus.Publish<FileLockChangedEvent, FileLockChangedMessage>(
                new FileLockChangedMessage(command.FileId, FileLockDto.ForUser(lockOwner)));
        }
    }
}