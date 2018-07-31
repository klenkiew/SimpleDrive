using FileService.Model;

namespace FileService.Services
{
    public interface ICurrentUser
    {
        string Id { get; }
        string Username { get; }
    }
    
    public static class CurrentUserExtensions
    {
        public static User ToDomainUser(this ICurrentUser currentUser)
        {
            return new User(currentUser.Id, currentUser.Username);
        }
    }
}