using System.Collections.Generic;
using System.Linq;
using FileService.Database;
using FileService.Database.EntityFramework;
using FileService.Dto;

namespace FileService.Queries
{
    public class FindFilesByUserQueryHandler : IQueryHandler<FindFilesByUserQuery, IEnumerable<FileDto>>
    {
        private readonly FileDbContext fileDb;

        public FindFilesByUserQueryHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb;
        }

        public IEnumerable<FileDto> Handle(FindFilesByUserQuery query)
        {
            return fileDb.Files
                .Where(file => file.Owner.Id == query.UserId || file.SharedWith.Any(sw => sw.UserId == query.UserId))
                .Select(file => new FileDto(file.Id, file.FileName, file.Description, file.Size, file.MimeType, 
                    file.DateCreated, file.DateModified, new UserDto(file.Owner.Id, file.Owner.Username, "N/A")))
                .ToList();
        }
    }
}