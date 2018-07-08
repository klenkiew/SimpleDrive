using System.Threading.Tasks;
using AuthenticationService.Services;
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
        public async Task<IActionResult> GetUsersByNamePrefix(string prefix)
        {
            return Ok(await usersService.GetUsersByNamePrefix(prefix ?? ""));
        }
    }
}