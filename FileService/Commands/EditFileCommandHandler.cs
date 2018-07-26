using System;
using System.Linq;
using EventBus;
using FileService.Database;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    internal class EditFileCommandHandler : ICommandHandler<EditFileCommand>
    {
        private readonly FileDbContext fileDb;
        private readonly ICurrentUser currentUser;
        private readonly IEventBusWrapper eventBus;

        public EditFileCommandHandler(FileDbContext fileDb, ICurrentUser currentUser, IEventBusWrapper eventBus)
        {
            this.fileDb = fileDb;
            this.currentUser = currentUser;
            this.eventBus = eventBus;
        }

        public void Handle(EditFileCommand command)
        {
            File file = fileDb.Files
                .Include(f => f.Owner)
                .Include(f => f.SharedWith)
                .FirstOrDefault(f => f.Id == command.FileId);

            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");
            
            if (currentUser.Id != file.OwnerId)
                throw new PermissionException($"The user doesn't have a permission to edit the file with id {command.FileId}");

            file.FileName = command.FileName;
            file.Description = command.Description;
            file.DateModified = DateTime.UtcNow;
            fileDb.SaveChanges();
            
            eventBus.Publish<FileEditedEvent, File>(file);
        }
    }
}