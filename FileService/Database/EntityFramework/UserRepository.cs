using System.Linq;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Database.EntityFramework
{
    public class UserRepository : IUserRepository
    {
        private readonly FileDbContext dbContext;

        public UserRepository(FileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public User GetById(string id)
        {
            return dbContext.Users
                .FirstOrDefault(f => f.Id == id);
        }

        public void Save(User entity)
        {
            dbContext.Users.Add(entity);
        }

        public void Update(User entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.Users.Attach(entity);
        }

        public void Delete(User entity)
        {
            dbContext.Users.Remove(entity);
        }
    }
}