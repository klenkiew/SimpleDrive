using System.IO;
using FileService.Model;

namespace FileService.Commands
{
    public class AddFileCommand
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public User Owner { get; set; }
        public Stream Content { get; set; }
    }
}