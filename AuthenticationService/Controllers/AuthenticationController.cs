using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationService.Dto;
using AuthenticationService.Model;
using AuthenticationService.Requests;
using AuthenticationService.Services;
using AuthenticationService.Validation;
using CommonEvents;
using EventBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IEmailConfirmationService emailConfirmationService;
        private readonly ITokenService tokenService;
        private readonly IEventBusWrapper eventBus;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IEmailConfirmationService emailConfirmationService, 
            ITokenService tokenService, 
            IEventBusWrapper eventBus,
            ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailConfirmationService = emailConfirmationService;
            this.tokenService = tokenService;
            this.eventBus = eventBus;
            this.logger = loggerFactory.CreateLogger<AuthenticationController>();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = new User()
            {
                Username = request.Username,
                Email = request.Email
            };
            var result = await userManager.CreateAsync(user, request.Password);
            
            if (!result.Succeeded) return BadRequest(new BasicError(result.Errors.First().Description));
            
            eventBus.Publish<IEvent<UserInfo>, UserInfo>(
                new UserRegisteredEvent(new UserInfo(user.Id, user.Username, user.Email)));

            try
            {
                await emailConfirmationService.SendConfirmationEmail(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send a confirmation e-mail.");
                var errorMessage = "The account has been successfully created, but the server failed to deliver " +
                                   "an account confirmation e-mail. Please try to use the resend option later" +
                                   " to activate your account.";
                return StatusCode(500, new BasicError(errorMessage));
            }

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
            
            var encodedToken = tokenService.BuildToken(user);
            return Ok(new JwtToken(encodedToken));
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return NotFound(new BasicError($"User with id {request.UserId} not found"));
            
            if (user.EmailConfirmed)
                return BadRequest(new BasicError($"User with id {request.UserId} already has a confirmed e-mail"));
            
            var result = await emailConfirmationService.ConfirmEmail(user, request.Token);
            
            if (!result.Succeeded) return BadRequest(new BasicError(result.Errors.First().Description));
            
            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return NotFound(new BasicError($"User with e-mail {request.Email} not found"));
            
            if (user.EmailConfirmed)
                return BadRequest(new BasicError($"User with e-mail {request.Email} already has a confirmed e-mail"));
            
            var passwordResult = await userManager.CheckPasswordAsync(user, request.Password);
            
            if (!passwordResult) 
                return BadRequest(new BasicError("Cannot resend confirmation e-mail: invalid password"));

            await emailConfirmationService.SendConfirmationEmail(user);
            
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await userManager.FindByIdAsync(GetCurrentUserId());
            var encodedToken = tokenService.BuildToken(user);
            return Ok(new JwtToken(encodedToken));
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var user = await userManager.FindByIdAsync(GetCurrentUserId());

            if (!await userManager.CheckPasswordAsync(user, request.Password))
                return BadRequest(new BasicError("Invalid password."));

            if (user.Email == request.Email)
                return BadRequest(new BasicError("The new e-mail address cannot be the same as your current e-mail."));                
            
            await emailConfirmationService.ProcessEmailChange(user, request.Email);
            
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeRequest request)
        {
            var user = await userManager.FindByIdAsync(GetCurrentUserId());

            var result = await emailConfirmationService.ConfirmEmailChange(user, request.Email, request.Token);
            
            if (!result.Succeeded) return BadRequest(new BasicError(result.Errors.First().Description));
            
            // the information in the user's token is stale after the e-mail change
            // - generate and send a new token with current user info  
            var encodedToken = tokenService.BuildToken(user);
            return Ok(new JwtToken(encodedToken));
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