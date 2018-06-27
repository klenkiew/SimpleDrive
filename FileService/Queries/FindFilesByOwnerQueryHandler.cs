using System.Collections.Generic;
using System.Linq;
using FileService.Database;
using FileService.Model;

namespace FileService.Queries
{
    public class FindFilesByOwnerQueryHandler : IQueryHandler<FindFilesByOwnerQuery, IEnumerable<File>>
    {
        private readonly FileDbContext fileDb;

        public FindFilesByOwnerQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public IEnumerable<File> Handle(FindFilesByOwnerQuery query)
        {
            return fileDb.Files.Where(file => file.Owner.Id == query.OwnerId).ToList();
        }
    }
}