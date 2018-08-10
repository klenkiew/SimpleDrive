using AuthenticationService.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Tests
{
    public class InMemoryDatabase
    {
        private static readonly DbContextOptions<UserDbContext> options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "User database")
            .Options;

        public static UserDbContext Get()
        {
            return new UserDbContext(options);
        }
    }
}