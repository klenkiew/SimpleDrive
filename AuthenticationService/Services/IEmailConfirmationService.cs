using System.Threading.Tasks;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public interface IEmailConfirmationService
    {
        Task SendConfirmationEmail(User user);
        Task<IdentityResult> ConfirmEmail(User user, string token);
        Task ProcessEmailChange(User user, string newEmail);
        Task<IdentityResult> ConfirmEmailChange(User user, string newEmail, string token);
    }
}