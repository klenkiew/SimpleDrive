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
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventBusWrapper eventBus;

        public AddFileCommandHandler(
            IFileRepository fileRepository,
            IUserRepository userRepository,
            IFileStorage fileStorage,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork,
            IEventBusWrapper eventBus)
        {
            this.fileRepository = fileRepository;
            this.userRepository = userRepository;
            this.fileStorage = fileStorage;
            this.currentUser = currentUser;
            this.unitOfWork = unitOfWork;
            this.eventBus = eventBus;
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
            
            // commit the transaction now so we don't end up with a file on disk but no entity in the database
            // which would mean there isn't a safe way to delete it
            unitOfWork.Commit();
            eventBus.Publish<FileAddedEvent, File>(file);
            
            fileStorage.SaveFile(file, command.Content);
        }
    }
}