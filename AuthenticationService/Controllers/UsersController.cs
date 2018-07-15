using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationService.Dto;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsersByNamePrefix(string prefix)
        {
            return await usersService.GetUsersByNamePrefix(prefix ?? "");
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            return await usersService.GetAllUsers();
        }
    }
}