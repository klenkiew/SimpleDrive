using FileService.Dto;
using FileService.Model;

namespace FileService.Queries
{
    public class FindFileByIdQuery : IQuery<FileDto>
    {
        public string FileId { get; }

        public FindFileByIdQuery(string fileId)
        {
            FileId = fileId;
        }
    }
}