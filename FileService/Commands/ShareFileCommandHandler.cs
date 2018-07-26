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
    internal class ShareFileCommandHandler : ICommandHandler<ShareFileCommand>
    {
        private readonly ICurrentUser currentUser;
        private readonly FileDbContext fileDb;
        private readonly IEventBusWrapper eventBus;

        public ShareFileCommandHandler(ICurrentUser currentUser, FileDbContext fileDb, IEventBusWrapper eventBus)
        {
            this.currentUser = currentUser;
            this.fileDb = fileDb;
            this.eventBus = eventBus;
        }

        public void Handle(ShareFileCommand command)
        {
            var file = fileDb.Files.Where(f => f.Id == command.FileId).Include(f => f.SharedWith).FirstOrDefault();
            
            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");
            
            if (currentUser.Id != file.OwnerId)
                throw new PermissionException($"The user doesn't have a permission to share the file with id {command.FileId}");

            if (command.ShareWithUserId == file.OwnerId)
                throw new PermissionException($"A file can't be shared with the owner.");
            
            if (!fileDb.Users.Any(u => u.Id == command.ShareWithUserId))
                throw new NotFoundException($"A user with id {command.ShareWithUserId} doesn't exist in the database.");

            var fileShare = new FileShare() {FileId = command.FileId, UserId = command.ShareWithUserId};
            file.SharedWith.Add(fileShare);
            fileDb.SaveChanges();
            
            eventBus.Publish<FileSharedEvent, FileSharesChangedMessage>(new FileSharesChangedMessage(file, fileShare));
        }
    }
}