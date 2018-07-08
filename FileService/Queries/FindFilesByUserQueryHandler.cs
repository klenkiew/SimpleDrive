using System.Collections.Generic;
using System.Linq;
using FileService.Database;
using FileService.Model;

namespace FileService.Queries
{
    public class FindFilesByUserQueryHandler : IQueryHandler<FindFilesByUserQuery, IEnumerable<File>>
    {
        private readonly FileDbContext fileDb;

        public FindFilesByUserQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public IEnumerable<File> Handle(FindFilesByUserQuery query)
        {
            return fileDb.Files
                .Where(file => file.Owner.Id == query.UserId || file.SharedWith.Any(sw => sw.UserId == query.UserId))
                .ToList();
        }
    }
}