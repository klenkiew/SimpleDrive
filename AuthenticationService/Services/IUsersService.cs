using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationService.Dto;

namespace AuthenticationService.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetUsersByNamePrefix(string namePrefix);
        Task<IEnumerable<UserDto>> GetAllUsers();
    }
}