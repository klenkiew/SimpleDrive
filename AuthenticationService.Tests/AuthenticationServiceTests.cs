using AuthenticationService.Dto;
using AuthenticationService.Model;
using AuthenticationService.Services;
using AuthenticationService.Tests.Mocks;
using CommonEvents;
using Moq;
using NUnit.Framework;

namespace AuthenticationService.Tests
{
    public class AuthenticationServiceTests
    {
        [Test]
        public void Register_creates_user_publishes_event_and_sends_email()
        {
            // Arrange
            ISignInManager signInManager = Mock.Of<ISignInManager>();
            ITokenService tokenService = Mock.Of<ITokenService>();
            
            var logger = new ConsoleLoggerMock<Services.AuthenticationService>();
            
            var userManager = new UserManagerMock();
            var eventBus = new EventPublisherMock();
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var service = new Services.AuthenticationService(
                userManager.Object, signInManager, tokenService, eventBus.Object, logger.Object, confirmationSender.Object);

            // Act
            OperationResult result = service.RegisterUser("username", "email", "password").Result;
            
            // Assert
            Assert.True(result.IsValid);
            userManager.VerifyUserCreated("username", "email", "password");
            eventBus.VerifyEventPublished<UserRegisteredEvent, UserInfo>(u => u.Username == "username" && u.Email == "email");
            confirmationSender.VerifyEmailSent("username", "email");
        }
        
        [Test]
        public void Create_token_succeeds_with_correct_email_and_password()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "email",
                Username = "username",
                PasswordHash = "password"
            };
            
            var signInManager = new SignInManagerMock();
            signInManager.SetupForUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<Services.AuthenticationService>();

            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var eventBus = new EventPublisherMock();
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var service = new Services.AuthenticationService(
                userManager.Object, signInManager.Object, tokenService.Object, eventBus.Object, logger.Object, 
                confirmationSender.Object);

            // Act
            OperationResult<JwtToken> result = service.CreateToken("email", "password").Result;
            
            // Assert
            Assert.True(result.IsValid);
            Assert.AreEqual("userId_token", result.Result.Token);
        }
        
        [Test]
        public void Refresh_token_succeeds_for_existing_user()
        {
            // Arrange
            var user = new User()
            {
                Id = "userId",
                Email = "email",
                Username = "username",
                PasswordHash = "password"
            };
            
            var signInManager = new SignInManagerMock();
            signInManager.SetupForUser(user);
            
            var tokenService = new TokenServiceMock();
            tokenService.SetupTokenBuild(u => u.Id + "_token");
            
            var logger = new ConsoleLoggerMock<Services.AuthenticationService>();

            var userManager = new UserManagerMock();
            userManager.SetupExistingUser(user);
            
            var eventBus = new EventPublisherMock();
            var confirmationSender = new EmailConfirmationSenderMock();
            
            var service = new Services.AuthenticationService(
                userManager.Object, signInManager.Object, tokenService.Object, eventBus.Object, logger.Object, 
                confirmationSender.Object);

            // Act
            OperationResult<JwtToken> result = service.RefreshToken("userId").Result;
            
            // Assert
            Assert.True(result.IsValid);
            Assert.AreEqual("userId_token", result.Result.Token);
        }
    }
}