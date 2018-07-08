using System.Collections.Generic;
using System.Linq;
using FileService.Database;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class FindUsersBySharedFileQueryHandler : IQueryHandler<FindUsersBySharedFileQuery, IEnumerable<User>>
    {
        private readonly FileDbContext fileDb;

        public FindUsersBySharedFileQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public IEnumerable<User> Handle(FindUsersBySharedFileQuery query)
        {
            return fileDb.Files
                .Where(file => file.Id == query.FileId)
                .Include(f => f.SharedWith)
                .ThenInclude(sharedWith => sharedWith.User)
                .First()
                .SharedWith
                .Select(sh => sh.User).ToList();
        }
    }
}