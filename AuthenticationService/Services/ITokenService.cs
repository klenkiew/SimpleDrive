using AuthenticationService.Model;

namespace AuthenticationService.Services
{
    public interface ITokenService
    {
        string BuildToken(User user);
    }
}