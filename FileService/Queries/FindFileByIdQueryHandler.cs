using System.Linq;
using FileService.Database;
using FileService.Dto;

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
            var file = fileDb.Files.FirstOrDefault(f => f.Id == query.FileId);
            return new FileDto(file.Id, file.FileName, file.Description, file.Size, file.MimeType, 
                file.DateCreated, file.DateModified, new UserDto(file.OwnerId, file.OwnerName, "N/A"));
        }
    }
}