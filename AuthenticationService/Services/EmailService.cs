using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AuthenticationService.Configuration;

namespace AuthenticationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailClientConfiguration clientConfiguration;

        public EmailService(EmailClientConfiguration clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration;
        }

        public async Task SendMail(string recipientAddress, string subject, string body)
        {
            using (var mail = new MailMessage(clientConfiguration.Address, recipientAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                using (var client = new SmtpClient
                {
                    Port = clientConfiguration.SmtpPort,
                    EnableSsl = clientConfiguration.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = clientConfiguration.SmtpHost,
                    Credentials = new NetworkCredential(clientConfiguration.Username, clientConfiguration.Password)
                })
                {
                    await client.SendMailAsync(mail);
                }
            }
        }
    }
}