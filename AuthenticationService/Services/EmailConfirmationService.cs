using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AuthenticationService.Configuration;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public class EmailConfirmationService : IEmailConfirmationService, IDisposable
    {
        private readonly RegistrationConfiguration configuration;
        private readonly UserManager<User> userManager;

        public EmailConfirmationService(RegistrationConfiguration configuration, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public async Task SendConfirmationEmail(User user)
        {
            if (user.EmailConfirmed)
                return;
            
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            
            string callbackQueryParams = $"?userId={user.Id}&&token={WebUtility.UrlEncode(token)}";
            string callbackUrl = configuration.CallbackUrl + callbackQueryParams;
            
            EmailClientConfiguration clientConfiguration = configuration.EmailClient;
            
            var mail = new MailMessage(clientConfiguration.Address, user.Email)
            {
                Subject = "Confirm your e-mail",
                Body = $"Please confirm your account by clicking the following link:\n{callbackUrl}"
            };
            
            var client = new SmtpClient
            {
                Port = clientConfiguration.SmtpPort,
                EnableSsl = clientConfiguration.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = clientConfiguration.SmtpHost,
                Credentials = new NetworkCredential(clientConfiguration.Username, clientConfiguration.Password)
            };

            await client.SendMailAsync(mail);
        }

        public Task<IdentityResult> ConfirmEmail(User user, string token)
        {
            return userManager.ConfirmEmailAsync(user, token);
        }

        public void Dispose()
        {
            userManager?.Dispose();
        }
    }
}