namespace FileService.Requests
{
    public class ShareFileRequest
    {
        public string FileId { get; }
        public string ShareWithUserId { get; }

        public ShareFileRequest(string fileId, string shareWithUserId)
        {
            FileId = fileId;
            ShareWithUserId = shareWithUserId;
        }
    }
}