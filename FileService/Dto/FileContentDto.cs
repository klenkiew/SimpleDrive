using System.IO;

namespace FileService.Dto
{
    public class FileContentDto
    {
        public string MimeType { get; set; }
        public Stream Content { get; set; }
    }
}