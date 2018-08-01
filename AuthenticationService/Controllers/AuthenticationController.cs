using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationService.Dto;
using AuthenticationService.Requests;
using AuthenticationService.Services;
using AuthenticationService.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAccountService accountService, IAuthenticationService authenticationService)
        {
            this.accountService = accountService;
            this.authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            OperationResult result = 
                await authenticationService.RegisterUser(request.Username, request.Email, request.Password);

            return ToActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginRequest request)
        {
            OperationResult<JwtToken> result = await authenticationService.CreateToken(request.Email, request.Password);

            return ToActionResult(result); 
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            OperationResult result = await accountService.ConfirmEmail(request.UserId, request.Token);

            return ToActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
        {
            OperationResult result = await accountService.ResendConfirmationEmail(request.Email, request.Password);
            
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            OperationResult<JwtToken> result = await authenticationService.RefreshToken(GetCurrentUserId());
            
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            OperationResult result = 
                await accountService.ChangeEmail(GetCurrentUserId(), request.Email, request.Password);

            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeRequest request)
        {
            OperationResult<JwtToken> result = 
                await accountService.ConfirmEmailChange(GetCurrentUserId(), request.Email, request.Token);

            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            OperationResult result =
                await accountService.ChangePassword(GetCurrentUserId(), request.CurrentPassword, request.NewPassword);

            return ToActionResult(result);
        }

        private string GetCurrentUserId()
        {
            return HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        }

        private IActionResult ToActionResult(OperationResult result)
        {
            if (result.IsValid)
                return Ok();

            OperationError firstError = result.Errors.FirstOrDefault();

            if (firstError == null)
                return BadRequest(new BasicError("An unknown error occured"));

            var errorDescription = new BasicError(firstError.Description);

            switch (firstError.Type)
            {
                case OperationError.ErrorType.General:
                    return BadRequest(errorDescription);
                case OperationError.ErrorType.NotFound:
                    return NotFound(errorDescription);
                case OperationError.ErrorType.ProcessingError:
                    return StatusCode(500, errorDescription);
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected [{typeof(OperationError.ErrorType).Name}] enum value");
            }
        }

        private IActionResult ToActionResult<T>(OperationResult<T> result)
        {
            return result.IsValid ? Ok(result.Result) : ToActionResult((OperationResult)result);
        }

    }
}