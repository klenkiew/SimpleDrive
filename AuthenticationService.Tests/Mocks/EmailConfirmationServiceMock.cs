using System.Threading.Tasks;
using AuthenticationService.Model;
using AuthenticationService.Services;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class EmailConfirmationServiceMock : Mock<IEmailConfirmationService>
    {
        public EmailConfirmationServiceMock()
        {
            Setup(service => service.ProcessEmailChange(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, email) => user.Email = email)
                .Returns(() => Task.CompletedTask);
            
            Setup(service => service.ConfirmEmailChange(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(OperationResult.Valid()));
        }

        public void SetupConfirmEmailFor(User user, string expectedToken)
        {
            Setup(service =>
                    service.ConfirmEmail(It.Is<User>(u => u.Id == user.Id),
                        It.Is<string>(token => token == expectedToken)))
                .Callback<User, string>((u, tok) => user.EmailConfirmed = true)
                .Returns(() => Task.FromResult(OperationResult.Valid()));
        }

        public void VerifyEmailChangeConfirmed(User user, string newEmail, string expectedToken)
        {
            Verify(service => service.ConfirmEmailChange(It.Is<User>(u => u.Id == user.Id),
                It.Is<string>(em => em == newEmail), It.Is<string>(tok => tok == expectedToken)));
        }
    }
}