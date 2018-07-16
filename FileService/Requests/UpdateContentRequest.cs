namespace FileService.Requests
{
    public class UpdateContentRequest
    {
        public string FileId { get; }
        public string Content { get; }

        public UpdateContentRequest(string fileId, string content)
        {
            FileId = fileId;
            Content = content;
        }
    }
}