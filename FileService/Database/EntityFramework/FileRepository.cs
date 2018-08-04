using System.Linq;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Database.EntityFramework
{
    public class FileRepository : IFileRepository
    {
        private readonly FileDbContext dbContext;

        public FileRepository(FileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public File GetById(string id)
        {
            return dbContext.Files
                .Include(f => f.Owner)
                .Include(f => f.SharedWith)
                    .ThenInclude(share => share.User)
                .FirstOrDefault(f => f.Id == id);
        }

        public void Save(File entity)
        {
            dbContext.Files.Add(entity);
        }

        public void Update(File entity)
        {
            dbContext.Update(entity);
        }

        public void Delete(File entity)
        {
            dbContext.Files.Remove(entity);
        }
    }
}