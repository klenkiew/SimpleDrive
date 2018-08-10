using System;
using AuthenticationService.Model;
using AuthenticationService.Services;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class TokenServiceMock : Mock<ITokenService>
    {
        public void SetupTokenBuild(Func<User, string> tokenGenerator)
        {
            this.Setup(service => service.BuildToken(It.IsAny<User>())).Returns(tokenGenerator);
        }
    }
}