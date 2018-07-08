namespace FileService.Commands
{
    public class UnshareFileCommand
    {
        public string FileId { get; set; }
        public string OwnerId { get; set; }
        public string UserId { get; set; }
    }
}