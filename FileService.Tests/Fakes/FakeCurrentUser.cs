using FileService.Services;

namespace FileService.Tests.Fakes
{
    public class FakeCurrentUser : ICurrentUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }
}