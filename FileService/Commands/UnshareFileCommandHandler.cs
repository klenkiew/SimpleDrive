using System.Linq;
using FileService.Database;
using Microsoft.EntityFrameworkCore;

namespace FileService.Commands
{
    internal class UnshareFileCommandHandler : ICommandHandler<UnshareFileCommand>
    {
        private readonly FileDbContext fileDb;

        public UnshareFileCommandHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public void Handle(UnshareFileCommand command)
        {
            var file = fileDb.Files
                .Where(f => f.Id == command.FileId)
                .Include(f => f.SharedWith)
                .ThenInclude(sh => sh.User)
                .FirstOrDefault();
            
            foreach (var fileShare in file.SharedWith)
            {
                if (fileShare.FileId == command.FileId && fileShare.UserId == command.UserId)
                {
                    file.SharedWith.Remove(fileShare);
                    break;
                }
            }

            fileDb.SaveChanges();
        }
    }
}