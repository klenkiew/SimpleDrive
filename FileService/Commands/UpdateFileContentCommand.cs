using System.IO;

namespace FileService.Commands
{
    public class UpdateFileContentCommand
    {
        public string FileId { get; }
        public Stream Content { get; }

        public UpdateFileContentCommand(string fileId, Stream content)
        {
            FileId = fileId;
            Content = content;
        }
    }
}