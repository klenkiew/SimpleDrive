using System.IO;
using FileService.Dto;
using FileService.Model;

namespace FileService.Queries
{
    public class GetFileContentQuery : IQuery<FileContentDto>
    {
        public string FileId { get; set; }
    }
}