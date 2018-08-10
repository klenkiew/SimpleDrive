using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthenticationService.Model;
using AuthenticationService.Services;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class EmailConfirmationSenderMock : Mock<IEmailConfirmationSender>
    {
        public EmailConfirmationSenderMock()
        {
            Setup(sender => sender.SendConfirmationEmail(It.IsAny<User>())).Returns(() => Task.CompletedTask);
        }

        public void VerifyEmailSent(string username, string email)
        {
            VerifyEmailSent(u => u.Username == username && u.Email == email);
        }

        public void VerifyEmailSent(Expression<Func<User, bool>> userMatch)
        {
            Verify(sender => sender.SendConfirmationEmail(It.Is(userMatch)));
        }
    }
}