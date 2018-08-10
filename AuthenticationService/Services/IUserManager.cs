using System.Threading.Tasks;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public interface IUserManager
    {
        Task<OperationResult> CreateAsync(User user, string password);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByIdAsync(string userId);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<OperationResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    }

    internal class UserManagerAdapter : IUserManager
    {
        private readonly UserManager<User> adaptee;

        public UserManagerAdapter(UserManager<User> adaptee)
        {
            this.adaptee = adaptee;
        }

        public async Task<OperationResult> CreateAsync(User user, string password)
        {
            return (await adaptee.CreateAsync(user, password)).ToOperationResult();
        }

        public Task<User> FindByEmailAsync(string email)
        {
            return adaptee.FindByEmailAsync(email);
        }

        public Task<User> FindByIdAsync(string userId)
        {
            return adaptee.FindByIdAsync(userId);
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            return adaptee.CheckPasswordAsync(user, password);
        }

        public async Task<OperationResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return (await adaptee.ChangePasswordAsync(user, currentPassword, newPassword)).ToOperationResult();
        }
    }
}