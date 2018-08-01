using System;
using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Commands
{
    internal class AddFileCommandHandler : ICommandHandler<AddFileCommand>
    {
        private readonly IFileRepository fileRepository;
        private readonly IUserRepository userRepository;
        private readonly IFileStorage fileStorage;
        private readonly ICurrentUser currentUser;
        private readonly IEventBusWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;
        
        public AddFileCommandHandler(
            IFileRepository fileRepository,
            IUserRepository userRepository,
            IFileStorage fileStorage,
            ICurrentUser currentUser,
            IEventBusWrapper eventBus, 
            IPostCommitRegistrator registrator)
        {
            this.fileRepository = fileRepository;
            this.userRepository = userRepository;
            this.fileStorage = fileStorage;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
            this.registrator = registrator;
        }

        public void Handle(AddFileCommand command)
        {
            User owner = userRepository.GetById(currentUser.Id);

            if (owner == null)
                throw new NotFoundException("The current user cannot be found in the database.");

            var dateCreated = DateTime.UtcNow;
            var size = command.Content.Length;
            var file = new File(command.FileName, command.Description, size, command.MimeType, dateCreated, owner);

            fileRepository.Save(file);
            eventBus.Publish<FileAddedEvent, File>(file);
            
            // save the file after commit so we don't end up with a file on disk but no entity in the database
            // which would mean there isn't a safe way to delete it
            registrator.Committed += () => fileStorage.SaveFile(file, command.Content);
        }
    }
}