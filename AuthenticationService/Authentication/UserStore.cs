﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationService.Database;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Authentication
{
    public class UserStore : IUserPasswordStore<User>, IUserEmailStore<User>
    {
        private readonly UserDbContext dbContext;

        public UserStore(UserDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Username);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(userName)) 
                throw new ArgumentException("Cannot be null or whitespace", nameof(userName));
            
            user.Username = userName;
            
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUsername);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedName)) 
                throw new ArgumentException("Cannot be null or whitespace", nameof(normalizedName));

            user.NormalizedUsername = normalizedName;
            
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            Debug.WriteLine($"User {user.Username} created.");
            
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return IdentityResult.Success;
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (string.IsNullOrWhiteSpace(userId)) 
                throw new ArgumentException("Cannot be null or whitespace", nameof(userId));

            var user = dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
            
            Debug.WriteLineIf(user == null, $"User with id {userId} not found.");
            
            return user;
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (string.IsNullOrWhiteSpace(normalizedUserName)) 
                throw new ArgumentException("Cannot be null or whitespace", nameof(normalizedUserName));

            var user = dbContext.Users.FirstOrDefaultAsync(
                u => u.NormalizedUsername == normalizedUserName, 
                cancellationToken: cancellationToken);
            
            Debug.WriteLineIf(user == null, $"User with name {normalizedUserName} not found.");
            
            return user;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Cannot be null or whitespace", nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (string.IsNullOrWhiteSpace(normalizedEmail)) 
                throw new ArgumentException("Cannot be null or whitespace", nameof(normalizedEmail));
            
            var user = dbContext.Users.FirstOrDefaultAsync(
                u => u.NormalizedEmail == normalizedEmail,
                cancellationToken: cancellationToken);
            
            Debug.WriteLineIf(user == null, $"User with email {normalizedEmail} not found.");
            
            return user;
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedEmail)) 
                throw new ArgumentException("Cannot be null or whitespace", nameof(normalizedEmail));

            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
    }
}