using System;
using System.IO;
using System.Linq;
using FileService.Database;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class GetFileContentQueryHandler : IQueryHandler<GetFileContentQuery, Stream>
    {
        private readonly FileDbContext fileDb;
        private readonly IFileStorage fileStorage;

        public GetFileContentQueryHandler(FileDbContext fileDb, IFileStorage fileStorage)
        {
            this.fileStorage = fileStorage;
            this.fileDb = fileDb;
        }

        public Stream Handle(GetFileContentQuery query)
        {
            var file = fileDb.Files
                .Where(f => f.Id == query.FileId)
                .Include(f => f.SharedWith).ThenInclude(sh => sh.User)
                .Include(f => f.Owner)
                .FirstOrDefault();
            
            return fileStorage.ReadFile(file).Result;
        }
    }
}