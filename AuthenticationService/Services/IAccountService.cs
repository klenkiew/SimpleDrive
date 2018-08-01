using System.Threading.Tasks;
using AuthenticationService.Dto;

namespace AuthenticationService.Services
{
    public interface IAccountService
    {
        Task<OperationResult> ConfirmEmail(string userId, string token);
        Task<OperationResult> ResendConfirmationEmail(string email, string password);
        Task<OperationResult> ChangeEmail(string userId, string email, string password);
        Task<OperationResult<JwtToken>> ConfirmEmailChange(string userId, string email, string token);
        Task<OperationResult> ChangePassword(string userId, string currentPassword, string newPassword);
    }
}