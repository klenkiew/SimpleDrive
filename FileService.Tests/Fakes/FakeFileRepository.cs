using System.Linq;
using FileService.Model;

namespace FileService.Tests.Fakes
{
    public class FakeFileRepository : FakeGenericRepository<File>, IFileRepository
    {
        protected override string EntityName { get; } = "file";

        public File GetByName(string filename)
        {
            return entitiesById.Values.FirstOrDefault(file => file.FileName == filename);
        }
    }
}