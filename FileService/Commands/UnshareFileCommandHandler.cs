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
    internal class UnshareFileCommandHandler : ICommandHandler<UnshareFileCommand>
    {
        private readonly ICurrentUser currentUser;
        private readonly FileDbContext fileDb;
        private readonly IEventBusWrapper eventBus;

        public UnshareFileCommandHandler(ICurrentUser currentUser, FileDbContext fileDb, IEventBusWrapper eventBus)
        {
            this.currentUser = currentUser;
            this.fileDb = fileDb;
            this.eventBus = eventBus;
        }

        public void Handle(UnshareFileCommand command)
        {
            File file = FindFile(command);
            FileShare shareToRemove = FindShareToRemove(command, file);

            file.SharedWith.Remove(shareToRemove);
            fileDb.SaveChanges();
            
            eventBus.Publish<FileSharedEvent, FileSharesChangedMessage>(new FileSharesChangedMessage(file, shareToRemove));
        }

        private static FileShare FindShareToRemove(UnshareFileCommand command, File file)
        {
            FileShare shareToRemove = file.SharedWith
                .FirstOrDefault(fileShare => fileShare.FileId == command.FileId && fileShare.UserId == command.UserId);

            if (shareToRemove == null)
                throw new NotFoundException($"Cannot unshare - a file with id {command.FileId} isn't shared with this user.");
            
            return shareToRemove;
        }

        private File FindFile(UnshareFileCommand command)
        {
            File file = fileDb.Files
                .Where(f => f.Id == command.FileId)
                .Include(f => f.SharedWith)
                .ThenInclude(sh => sh.User)
                .FirstOrDefault();

            if (file == null)
                throw new NotFoundException($"A file with id {command.FileId} doesn't exist in the database.");

            if (!HasPermissionToUnshare(command, file))
                throw new PermissionException($"The user doesn't have a permission to unshare the file with id {command.FileId}");
            
            return file;
        }

        private bool HasPermissionToUnshare(UnshareFileCommand command, File file)
        {
            return currentUser.Id == file.OwnerId || command.UserId == currentUser.Id;
        }
    }
}