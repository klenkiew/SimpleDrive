using System.Linq;
using FileService.Model;
using FileService.Tests.Helpers;

namespace FileService.Tests.Fakes
{
    public class FakeFileRepository : FakeGenericRepository<File>, IFileRepository
    {
        protected override string EntityName { get; } = "file";

        public override void Save(File entity, string id)
        {
            EntityHelper.SetId(entity, id);
            base.Save(entity, id);
        }

        public File GetByName(string filename)
        {
            return filesById.Values.FirstOrDefault(file => file.FileName == filename);
        }
    }
}