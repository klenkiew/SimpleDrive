using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FileService.Services
{
    internal class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string Id => httpContextAccessor.HttpContext.User.Claims
            .First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

        public string Username => httpContextAccessor.HttpContext.User.Identity.Name;
    }
}