using System;
using System.Threading.Tasks;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthenticationService.Services
{
    /// <summary>
    /// A mock implementation of the interface for the development environment
    /// to avoid the need for confirming all registered accounts with a valid e-mail address. 
    /// </summary>
    /// <remarks>
    /// The client UI assumes that e-mail confirmation is required and the messages presented to the user clearly
    /// indicate that this step is necessary, e.g "Please now confirm your e-mail address" after registering an account,
    /// so the messages sometimes may be confusing when this implementation is used.
    /// </remarks>
    internal class EmptyEmailConfirmationService : IEmailConfirmationService, IEmailConfirmationSender, IDisposable
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<EmptyEmailConfirmationService> logger;
        
        public EmptyEmailConfirmationService(UserManager<User> userManager, ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            this.logger = loggerFactory.CreateLogger<EmptyEmailConfirmationService>();
        }


        public Task SendConfirmationEmail(User user)
        {
            // Do nothing - this implementation is used when the e-mail confirmation feature is disabled.
            return Task.CompletedTask;
        }

        public Task<IdentityResult> ConfirmEmail(User user, string token)
        {
            throw new InvalidOperationException("The e-mail confirmation feature is currently disabled.");
        }

        public async Task ProcessEmailChange(User user, string newEmail)
        {
            // just change the user's e-mail - no confirmation etc.
            var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            await userManager.ChangeEmailAsync(user, newEmail, token);
        }

        public Task<IdentityResult> ConfirmEmailChange(User user, string newEmail, string token)
        {
            throw new InvalidOperationException("The e-mail confirmation feature is currently disabled.");
        }

        public void Dispose()
        {
            userManager?.Dispose();
        }
    }
}