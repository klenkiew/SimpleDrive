using System.Linq;
using FileService.Database;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Services;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class GetFileContentQueryHandler : IQueryHandler<GetFileContentQuery, FileContentDto>
    {
        private readonly FileDbContext fileDb;
        private readonly IFileStorage fileStorage;

        public GetFileContentQueryHandler(FileDbContext fileDb, IFileStorage fileStorage)
        {
            this.fileStorage = fileStorage;
            this.fileDb = fileDb;
        }

        public FileContentDto Handle(GetFileContentQuery query)
        {
            var file = fileDb.Files
                .Where(f => f.Id == query.FileId)
                .Include(f => f.SharedWith).ThenInclude(sh => sh.User)
                .Include(f => f.Owner)
                .FirstOrDefault();

            var content = fileStorage.ReadFile(file).Result;
            return new FileContentDto(file.MimeType, content);
        }
    }
}