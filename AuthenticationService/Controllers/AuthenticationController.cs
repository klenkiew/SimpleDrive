using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationService.Model;
using AuthenticationService.Requests;
using AuthenticationService.Services;
using AuthenticationService.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly TokenService tokenService;

        public AuthenticationController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            TokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var identityUser = new User()
            {
                UserName = request.Username,
                Email = request.Email
            };
            var result = await userManager.CreateAsync(identityUser, request.Password);
            
            if (!result.Succeeded) return BadRequest(new BasicError(result.Errors.First().Description));
            
            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return BadRequest(new BasicError("Login failed: invalid e-mail or password."));
            
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            
            if (!result.Succeeded) 
                return BadRequest(ExtractError(result));
            
            var tokenString = tokenService.BuildToken(user);
            return Ok(new {token = tokenString});
        }
        
        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await userManager.FindByIdAsync(GetCurrentUserId());
            var tokenString = tokenService.BuildToken(user);
            return Ok(new {token = tokenString});
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var user = await userManager.FindByIdAsync(GetCurrentUserId());

            if (!await userManager.CheckPasswordAsync(user, request.Password))
                return BadRequest(new BasicError("Invalid password."));
            
            var token = await userManager.GenerateChangeEmailTokenAsync(user, request.Email);
            var result = await userManager.ChangeEmailAsync(user, request.Email, token);
            
            if (!result.Succeeded) return BadRequest(new BasicError(result.Errors.First().Description));
            
            // the information in the user's token is stale after the e-mail change
            // - generate and send a new token with current user info  
            var tokenString = tokenService.BuildToken(user);
            return Ok(new {token = tokenString});
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await userManager.FindByIdAsync(
                HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            
            if (!result.Succeeded) return BadRequest(new BasicError(result.Errors.First().Description));
            
            return Ok();
        }

        private string GetCurrentUserId()
        {
            return HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        }

        private static BasicError ExtractError(SignInResult result)
        {
            var details = "";
            if (result.IsLockedOut)
                details = "the account is locked out.";
            else if (result.RequiresTwoFactor)
                details = "two factor authentication is required.";
            else if (result.IsNotAllowed)
                details = "invalid e-mail or password.";
            return new BasicError("Login failed: " + details);
        }
    }
}