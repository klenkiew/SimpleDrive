using System.Threading.Tasks;
using AuthenticationService.Model;
using AuthenticationService.Services;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class SignInManagerMock : Mock<ISignInManager>
    {
        public void SetupForUser(User user)
        {
            Setup(manager => manager.CheckPasswordSignInAsync(
                    It.Is<User>(u => u.Id == user.Id), It.Is<string>(p => p == user.PasswordHash), It.IsAny<bool>()))
                .Returns(() => Task.FromResult(OperationResult.Valid()));
        }
    }
}