using System.Threading.Tasks;
using AuthenticationService.Dto;

namespace AuthenticationService.Services
{
    public interface IAuthenticationService
    {
        Task<OperationResult> RegisterUser(string username, string email, string password);
        Task<OperationResult<JwtToken>> CreateToken(string email, string password);
        Task<OperationResult<JwtToken>> RefreshToken(string userId);
    }
}