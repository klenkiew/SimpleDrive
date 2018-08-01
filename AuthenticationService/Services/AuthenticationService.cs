﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticationService.Dto;
using AuthenticationService.Model;
using CommonEvents;
using EventBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthenticationService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenService tokenService;
        private readonly IEventBusWrapper eventBus;
        private readonly ILogger<AuthenticationService> logger;
        private readonly IEmailConfirmationSender emailConfirmationService;

        public AuthenticationService(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            ITokenService tokenService, 
            IEventBusWrapper eventBus, 
            ILogger<AuthenticationService> logger, 
            IEmailConfirmationSender emailConfirmationService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.eventBus = eventBus;
            this.logger = logger;
            this.emailConfirmationService = emailConfirmationService;
        }


        public async Task<OperationResult> RegisterUser(string username, string email, string password)
        {
            var user = new User()
            {
                Username = username,
                Email = email
            };

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded) return result.ToOperationResult();

            eventBus.Publish<UserRegisteredEvent, UserInfo>(new UserInfo(user.Id, user.Username, user.Email));

            try
            {
                await emailConfirmationService.SendConfirmationEmail(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send a confirmation e-mail.");
                const string errorMessage =
                    "The account has been successfully created, but the server failed to deliver " +
                    "an account confirmation e-mail. Please try to use the resend option later" +
                    " to activate your account.";
                return OperationResult.Invalid(new List<OperationError>()
                {
                    new OperationError(errorMessage, OperationError.ErrorType.ProcessingError)
                });
            }

            return OperationResult.Valid();
        }

        public async Task<OperationResult<JwtToken>> CreateToken(string email, string password)
        {
            User user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return OperationResult<JwtToken>.Invalid("Login failed: invalid e-mail or password.");

            SignInResult result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return result.ToOperationResult().Cast<JwtToken>();

            var encodedToken = tokenService.BuildToken(user);
            return OperationResult<JwtToken>.Valid(new JwtToken(encodedToken));
        }

        public async Task<OperationResult<JwtToken>> RefreshToken(string userId)
        {
            User user = await userManager.FindByIdAsync(userId);
            var encodedToken = tokenService.BuildToken(user);
            return OperationResult<JwtToken>.Valid(new JwtToken(encodedToken));
        }
    }
}