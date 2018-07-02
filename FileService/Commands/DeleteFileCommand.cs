using FileService.Model;

namespace FileService.Commands
{
    public class DeleteFileCommand
    {
        public string FileId { get; set; }
        public User Owner { get; set; }
    }
}