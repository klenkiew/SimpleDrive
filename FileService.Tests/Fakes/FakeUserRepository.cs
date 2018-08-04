using FileService.Model;

namespace FileService.Tests.Fakes
{
    public class FakeUserRepository : FakeGenericRepository<User>, IUserRepository
    {
        protected override string EntityName { get; } = "user";
    }
}