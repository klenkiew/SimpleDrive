using System.IO;

namespace FileService.Commands
{
    public class UpdateFileContentCommand
    {
        public string FileId { get; set; }
        public Stream Content { get; set; }
    }
}