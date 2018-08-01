using System.Linq;
using FileService.Database.EntityFramework;
using FileService.Dto;
using FileService.Model;
using Microsoft.EntityFrameworkCore;

namespace FileService.Queries
{
    public class FindFileByIdQueryHandler : IQueryHandler<FindFileByIdQuery, FileDto>
    {
        private readonly FileDbContext fileDb;

        public FindFileByIdQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public FileDto Handle(FindFileByIdQuery query)
        {
            File file = fileDb.Files
                .Include(f => f.Owner)
                .FirstOrDefault(f => f.Id == query.FileId)
                .EnsureFound(query.FileId);
            
            return new FileDto(file.Id, file.FileName, file.Description, file.Size, file.MimeType, 
                file.DateCreated, file.DateModified, new UserDto(file.Owner.Id, file.Owner.Username, "N/A"));
        }
    }
}