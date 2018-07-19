using System.Threading.Tasks;

namespace AuthenticationService.Services
{
    public interface IEmailService
    {
        Task SendMail(string recipientAddress, string subject, string body);
    }
}