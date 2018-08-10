using System.Threading.Tasks;
using AuthenticationService.Model;
using AuthenticationService.Services;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class UserManagerMock : Mock<IUserManager>
    {
        public UserManagerMock()
        {
            Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(OperationResult.Valid()));
        }

        public void SetupExistingUser(User user)
        {
            Setup(manager => manager.FindByEmailAsync(It.Is<string>(email => email == user.Email)))
                .Returns(() => Task.FromResult(user));
            
            Setup(manager => manager.FindByIdAsync(It.Is<string>(id => id == user.Id)))
                .Returns(() => Task.FromResult(user));

            Setup(manager =>
                manager.CheckPasswordAsync(It.Is<User>(u => u.Id == user.Id), It.Is<string>(p => p == user.PasswordHash)))
                .Returns(() => Task.FromResult(true));
            
            Setup(manager =>
                    manager.ChangePasswordAsync(It.Is<User>(u => u.Id == user.Id), It.Is<string>(p => p == user.PasswordHash), It.IsAny<string>()))
                .Callback<User, string, string>((u, oldPassword, newPassword) => u.PasswordHash = newPassword)
                .Returns(() => Task.FromResult(OperationResult.Valid()));
        }

        public void VerifyUserCreated(string username, string email, string password)
        {
            Verify(manager =>
                manager.CreateAsync(It.Is<User>(user => user.Username == username && user.Email == email), password));
        }
    }
}