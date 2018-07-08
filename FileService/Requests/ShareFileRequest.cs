namespace FileService.Requests
{
    public class ShareFileRequest
    {
        public string FileId { get; set; }
        public string ShareWithUserId { get; set; }
    }
}