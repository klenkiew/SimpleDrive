using FileService.Model;

namespace FileService.Infrastructure
{
    public interface ICurrentUserSource
    {
        User GetCurrentUser();
    }
}