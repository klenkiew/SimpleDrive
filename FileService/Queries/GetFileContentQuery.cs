using System.IO;
using FileService.Model;

namespace FileService.Queries
{
    public class GetFileContentQuery : IQuery<Stream>
    {
        public string FileId { get; set; }
    }
}