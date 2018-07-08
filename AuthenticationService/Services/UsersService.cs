using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationService.Database;
using AuthenticationService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Services
{
    internal class UsersService : IUsersService
    {
        private readonly UserDbContext db;

        public UsersService(UserDbContext dbContext)
        {
            this.db = dbContext;
        }

        public async Task<IEnumerable<User>> GetUsersByNamePrefix(string namePrefix)
        {
            return await db.Users
                .Where(u => u.Username.StartsWith(namePrefix, StringComparison.CurrentCultureIgnoreCase))
                .ToListAsync();
        }
    }
}