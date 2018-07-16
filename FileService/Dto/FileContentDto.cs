using System.IO;

namespace FileService.Dto
{
    public class FileContentDto
    {
        public string MimeType { get; }
        public Stream Content { get; }

        public FileContentDto(string mimeType, Stream content)
        {
            MimeType = mimeType;
            Content = content;
        }
    }
}