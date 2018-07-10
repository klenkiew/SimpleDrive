using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationService.Database;
using AuthenticationService.Dto;
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

        public async Task<IEnumerable<UserDto>> GetUsersByNamePrefix(string namePrefix)
        {
            return await UsersAsDto()
                .Where(u => u.Username.StartsWith(namePrefix, StringComparison.CurrentCultureIgnoreCase))
                .ToListAsync();
        }
        
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            return await UsersAsDto().ToListAsync();
        }

        private IQueryable<UserDto> UsersAsDto()
        {
            return db.Users.Select(u => new UserDto(u.Id, u.Username, u.Email));
        }
    }
}