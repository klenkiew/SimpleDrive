using System.Linq;
using FileService.Database;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    internal class ShareFileCommandHandler : ICommandHandler<ShareFileCommand>
    {
        private readonly FileDbContext fileDb;

        public ShareFileCommandHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public void Handle(ShareFileCommand command)
        {
            var file = fileDb.Files.Where(f => f.Id == command.FileId).Include(f => f.SharedWith).FirstOrDefault();
            file.SharedWith.Add(new FileShare() {FileId = command.FileId, UserId = command.SharedWithUserId});
            fileDb.SaveChanges();
        }
    }
}