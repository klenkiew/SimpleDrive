namespace FileService.Services
{
    public interface ICurrentUser
    {
        string Id { get; }
        string Username { get; }
    }
}