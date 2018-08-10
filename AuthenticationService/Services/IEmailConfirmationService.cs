using System.Threading.Tasks;
using AuthenticationService.Model;

namespace AuthenticationService.Services
{
    public interface IEmailConfirmationService
    {
        Task<OperationResult> ConfirmEmail(User user, string token);        
        Task ProcessEmailChange(User user, string newEmail);
        Task<OperationResult> ConfirmEmailChange(User user, string newEmail, string token);
    }

    public interface IEmailConfirmationSender
    {
        Task SendConfirmationEmail(User user);
    }
}