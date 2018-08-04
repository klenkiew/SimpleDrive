using FileService.Model;
using FileService.Services;

namespace FileService.Infrastructure
{
    public class CurrentUserSource : ICurrentUserSource
    {
        private readonly IUserRepository userRepository;
        private readonly ICurrentUser currentUser;

        public CurrentUserSource(IUserRepository userRepository, ICurrentUser currentUser)
        {
            this.userRepository = userRepository;
            this.currentUser = currentUser;
        }

        public User GetCurrentUser()
        {
            return userRepository.GetById(currentUser.Id);
        }
    }
}