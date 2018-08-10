using System.Threading.Tasks;
using AuthenticationService.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public interface ISignInManager
    {
        Task<OperationResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);
    }

    internal class SignInManagerAdapter : ISignInManager
    {
        private readonly SignInManager<User> adaptee;

        public SignInManagerAdapter(SignInManager<User> adaptee)
        {
            this.adaptee = adaptee;
        }

        public async Task<OperationResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            return (await adaptee.CheckPasswordSignInAsync(user, password, lockoutOnFailure)).ToOperationResult();
        }
    }
}