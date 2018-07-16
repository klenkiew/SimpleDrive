using FileService.Dto;

namespace FileService.Queries
{
    public class GetFileContentQuery : IQuery<FileContentDto>
    {
        public string FileId { get; }

        public GetFileContentQuery(string fileId)
        {
            FileId = fileId;
        }
    }
}