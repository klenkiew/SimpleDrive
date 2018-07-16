using System.IO;

namespace FileService.Commands
{
    public class AddFileCommand
    {
        public string FileName { get; }
        public string Description { get; }
        public string MimeType { get; }
        public Stream Content { get; }

        public AddFileCommand(string fileName, string description, string mimeType, Stream content)
        {
            FileName = fileName;
            Description = description;
            MimeType = mimeType;
            Content = content;
        }
    }
}