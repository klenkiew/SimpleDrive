using System.Linq;
using FileService.Database;
using FileService.Model;

namespace FileService.Queries
{
    public class FindFileByIdQueryHandler : IQueryHandler<FindFileByIdQuery, File>
    {
        private readonly FileDbContext fileDb;

        public FindFileByIdQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public File Handle(FindFileByIdQuery query)
        {
            return fileDb.Files.FirstOrDefault(file => file.Id == query.FileId);
        }
    }
}