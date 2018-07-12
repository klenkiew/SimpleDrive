using FileService.Dto;

namespace FileService.Queries
{
    public class GetFileLockQuery : IQuery<FileLockDto>
    {
        public string FileId { get; set; }
    }
}