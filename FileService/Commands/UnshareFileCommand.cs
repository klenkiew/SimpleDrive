namespace FileService.Commands
{
    public class UnshareFileCommand
    {
        public string FileId { get; }
        public string UserId { get; }

        public UnshareFileCommand(string fileId, string userId)
        {
            FileId = fileId;
            UserId = userId;
        }
    }
}