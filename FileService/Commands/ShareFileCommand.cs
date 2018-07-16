namespace FileService.Commands
{
    public class ShareFileCommand
    {
        public string FileId { get; }
        public string ShareWithUserId { get; }

        public ShareFileCommand(string fileId, string shareWithUserId)
        {
            FileId = fileId;
            ShareWithUserId = shareWithUserId;
        }
    }
}