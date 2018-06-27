using System.IO;

namespace FileService.Commands
{
    public class AddFileCommand
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
//        public int Size { get; set; }
//        public string PhysicalPath { get; set; }
        public string OwnerUserId { get; set; }
        public Stream Content { get; set; }
    }
}