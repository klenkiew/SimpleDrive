using System.Threading.Tasks;
using AuthenticationService.Dto;
using AuthenticationService.Model;
using Microsoft.Extensions.Logging;

namespace AuthenticationService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserManager userManager;
        private readonly ITokenService tokenService;
        private readonly ILogger<AccountService> logger;
        private readonly IEmailConfirmationService emailConfirmationService;
        private readonly IEmailConfirmationSender emailConfirmationSender;

        public AccountService(
            IUserManager userManager,
            IEmailConfirmationService emailConfirmationService,
            IEmailConfirmationSender emailConfirmationSender,
            ITokenService tokenService,
            ILogger<AccountService> logger)
        {
            this.userManager = userManager;
            this.emailConfirmationService = emailConfirmationService;
            this.emailConfirmationSender = emailConfirmationSender;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        public async Task<OperationResult> ConfirmEmail(string userId, string token)
        {
            User user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return OperationResult.Invalid(
                    new OperationError($"User with id {userId} not found", OperationError.ErrorType.NotFound));
            }

            if (user.EmailConfirmed)
                return OperationResult.Invalid($"User with id {userId} already has a confirmed e-mail");

            OperationResult result = await emailConfirmationService.ConfirmEmail(user, token);
            return result;
        }

        public async Task<OperationResult> ResendConfirmationEmail(string email, string password)
        {
            User user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return OperationResult.Invalid(
                    new OperationError($"User with e-mail {email} not found", OperationError.ErrorType.NotFound));
            }

            if (user.EmailConfirmed)
                return OperationResult.Invalid($"User with e-mail {email} already has a confirmed e-mail");

            var passwordResult = await userManager.CheckPasswordAsync(user, password);
            
            if (!passwordResult) 
                return OperationResult.Invalid("Cannot resend confirmation e-mail: invalid password");

            await emailConfirmationSender.SendConfirmationEmail(user);
            
            return OperationResult.Valid();
        }

        public async Task<OperationResult> ChangeEmail(string userId, string email, string password)
        {
            User user = await userManager.FindByIdAsync(userId);

            if (!await userManager.CheckPasswordAsync(user, password))
                return OperationResult.Invalid("Invalid password");

            if (user.Email == email)
                return OperationResult.Invalid("The new e-mail address cannot be the same as your current e-mail.");
            
            await emailConfirmationService.ProcessEmailChange(user, email);
            
            return OperationResult.Valid();
        }
        
        public async Task<OperationResult<JwtToken>> ConfirmEmailChange(string userId, string email, string token)
        {
            User user = await userManager.FindByIdAsync(userId);

            OperationResult result = await emailConfirmationService.ConfirmEmailChange(user, email, token);
            
            if (!result.IsValid) return result.Cast<JwtToken>();
            
            // the information in the user's token is stale after the e-mail change
            // - generate and send a new token with current user info  
            var encodedToken = tokenService.BuildToken(user);
            return OperationResult<JwtToken>.Valid(new JwtToken(encodedToken));
        }
        
        public async Task<OperationResult> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            User user = await userManager.FindByIdAsync(userId);

            return await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
    }
}