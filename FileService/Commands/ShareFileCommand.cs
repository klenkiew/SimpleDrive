namespace FileService.Commands
{
    public class ShareFileCommand
    {
        public string OwnerId { get; set; }
        public string FileId { get; set; }
        public string SharedWithUserId { get; set; }
    }
}