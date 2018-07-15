using System.IO;

namespace FileService.Commands
{
    public class AddFileCommand
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        public string MimeType { get; set; }
        public Stream Content { get; set; }
    }
}