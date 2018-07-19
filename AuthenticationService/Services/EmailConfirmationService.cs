using System;
using System.Net;
using System.Threading.Tasks;
using AuthenticationService.Configuration;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public class EmailConfirmationService : IEmailConfirmationService, IDisposable
    {
        private readonly IEmailService emailService;
        private readonly UserManager<User> userManager;
        private readonly EmailConfirmationConfiguration configuration;

        public EmailConfirmationService(
            IEmailService emailService, 
            UserManager<User> userManager, 
            EmailConfirmationConfiguration configuration)
        {
            this.emailService = emailService;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task SendConfirmationEmail(User user)
        {
            if (user.EmailConfirmed)
                throw new InvalidOperationException("The user already has a confirmed e-mail.");
            
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            
            var callbackQueryParams = $"?userId={user.Id}&&token={WebUtility.UrlEncode(token)}";
            var callbackUrl = configuration.ConfirmEmailCallbackUrl + callbackQueryParams;

            var subject = "Confirm your e-mail";
            var body = $"Please confirm your account by clicking the following link:\n{callbackUrl}";
            
            await emailService.SendMail(user.Email, subject, body);
        }

        public async Task ProcessEmailChange(User user, string newEmail)
        {
            await SendEmailChangeNotificationEmail(user, newEmail);
            await SendEmailChangeConfirmationEmail(user, newEmail);
        }

        public Task<IdentityResult> ConfirmEmail(User user, string token)
        {
            return userManager.ConfirmEmailAsync(user, token);
        }

        public Task<IdentityResult> ConfirmEmailChange(User user, string newEmail, string token)
        {
            return userManager.ChangeEmailAsync(user, newEmail, token);
        }

        public void Dispose()
        {
            userManager?.Dispose();
        }

        private async Task SendEmailChangeConfirmationEmail(User user, string newEmail)
        {
            var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            var callbackQueryParams = $"?email={WebUtility.UrlEncode(newEmail)}&&token={WebUtility.UrlEncode(token)}";
            var callbackUrl = configuration.ConfirmEmailChangeCallbackUrl + callbackQueryParams;

            var subject = "Confirm e-mail change";
            var body = $"Please confirm your e-mail by clicking the following link:\n{callbackUrl}";

            await emailService.SendMail(newEmail, subject, body);
        }

        private async Task SendEmailChangeNotificationEmail(User user, string newEmail)
        {
            var subject = "You have requested an e-mail address change";
            var body = "As soon as you confirm your new e-mail, your account will be assigned " +
                       $"to the following e-mail address: {newEmail}";
            
            await emailService.SendMail(user.Email, subject, body);
        }
    }
}