using AuthenticationService.Model;
using AuthenticationService.Services;
using AuthenticationService.Tests.Mocks;
using NUnit.Framework;

namespace AuthenticationService.Tests
{
    public class AccountServiceTests
    {
        [Test]
        public void After_confirm_email_call_users_email_is_confirmed()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "mail@mail.com",
                EmailConfirmed = false
            };
            
            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<AccountService>();
            
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var emailConfirmationService = new EmailConfirmationServiceMock();
            emailConfirmationService.SetupConfirmEmailFor(user, "userId_token");
            
            
            var service = new AccountService(
                userManager.Object, emailConfirmationService.Object, confirmationSender.Object, tokenService.Object, logger.Object);

            // Act
            OperationResult result = service.ConfirmEmail("userId", "userId_token").Result;
            
            // Assert
            Assert.True(result.IsValid);
            Assert.True(user.EmailConfirmed);
        }
        
        [Test]
        public void Email_confirmation_can_be_resent()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "mail@mail.com",
                EmailConfirmed = false,
                PasswordHash = "password",
                Username = "username"
            };
            
            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<AccountService>();
            
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var emailConfirmationService = new EmailConfirmationServiceMock();
            emailConfirmationService.SetupConfirmEmailFor(user, "userId_token");
            
            var service = new AccountService(
                userManager.Object, emailConfirmationService.Object, confirmationSender.Object, tokenService.Object, logger.Object);

            // Act
            OperationResult result = service.ResendConfirmationEmail("mail@mail.com", "password").Result;
            
            // Assert
            Assert.True(result.IsValid);
            confirmationSender.VerifyEmailSent("username", "mail@mail.com");
        }
        
        [Test]
        public void Users_email_can_be_changed()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "mail@mail.com",
                EmailConfirmed = true,
                PasswordHash = "password",
                Username = "username"
            };
            
            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<AccountService>();
            
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var emailConfirmationService = new EmailConfirmationServiceMock();
            emailConfirmationService.SetupConfirmEmailFor(user, "userId_token");
            
            var service = new AccountService(
                userManager.Object, emailConfirmationService.Object, confirmationSender.Object, tokenService.Object, logger.Object);

            // Act
            OperationResult result = service.ChangeEmail("userId", "newmail@mail.com", "password").Result;
            
            // Assert
            Assert.True(result.IsValid);
            Assert.AreEqual("newmail@mail.com", user.Email);
        }
        
        [Test]
        public void Users_password_can_be_changed()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "mail@mail.com",
                EmailConfirmed = true,
                PasswordHash = "password",
                Username = "username"
            };
            
            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<AccountService>();
            
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var emailConfirmationService = new EmailConfirmationServiceMock();
            emailConfirmationService.SetupConfirmEmailFor(user, "userId_token");
            
            var service = new AccountService(
                userManager.Object, emailConfirmationService.Object, confirmationSender.Object, tokenService.Object, logger.Object);

            // Act
            OperationResult result = service.ChangePassword("userId", "password", "newPassword").Result;
            
            // Assert
            Assert.True(result.IsValid);
            Assert.AreEqual("newPassword", user.PasswordHash);
        }
        
        [Test]
        public void Email_change_can_be_confirmed()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "mail@mail.com",
                EmailConfirmed = true,
                PasswordHash = "password",
                Username = "username"
            };
            
            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<AccountService>();
            
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var emailConfirmationService = new EmailConfirmationServiceMock();
            emailConfirmationService.SetupConfirmEmailFor(user, "userId_token");
            
            var service = new AccountService(
                userManager.Object, emailConfirmationService.Object, confirmationSender.Object, tokenService.Object, logger.Object);

            // Act
            OperationResult result = service.ConfirmEmailChange("userId", "newmail@mail.com", "userId_token").Result;
            
            // Assert
            Assert.True(result.IsValid);
            emailConfirmationService.VerifyEmailChangeConfirmed(user, "newmail@mail.com", "userId_token");
        }
    }
}