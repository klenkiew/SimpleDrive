using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationService.Model;

namespace AuthenticationService.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetUsersByNamePrefix(string namePrefix);
    }
}