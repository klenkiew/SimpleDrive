using FileService.Dto;

namespace FileService.Queries
{
    public class GetFileLockQuery : IQuery<FileLockDto>
    {
        public GetFileLockQuery(string fileId)
        {
            FileId = fileId;
        }

        public string FileId { get; }
    }
}