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
    public class AddFileCommandHandler : ICommandHandler<AddFileCommand>
    {
        private readonly IFileRepository fileRepository;
        private readonly IFileStorage fileStorage;
        private readonly ICurrentUserSource currentUserSource;
        private readonly IPublisherWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;
        
        public AddFileCommandHandler(
            IFileRepository fileRepository,
            IFileStorage fileStorage,
            IPublisherWrapper eventBus, 
            IPostCommitRegistrator registrator, 
            ICurrentUserSource currentUserSource)
        {
            this.fileRepository = fileRepository;
            this.fileStorage = fileStorage;
            this.eventBus = eventBus;
            this.registrator = registrator;
            this.currentUserSource = currentUserSource;
        }

        public void Handle(AddFileCommand command)
        {
            User owner = currentUserSource.GetCurrentUser();

            if (owner == null)
                throw new NotFoundException("The current user cannot be found in the database.");

            var dateCreated = DateTime.UtcNow;
            var size = command.Content.Length;
            var file = new File(command.FileName, command.Description, size, command.MimeType, dateCreated, owner);

            fileRepository.Save(file);
            
            registrator.Committed += () =>
            {
                eventBus.Publish<FileAddedEvent, File>(file);
                // save the file after commit so we don't end up with a file on disk but no entity in the database
                // which would mean there isn't a safe way to delete it
                fileStorage.SaveFile(file, command.Content);
            };
        }
    }
}