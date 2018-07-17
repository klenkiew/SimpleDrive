namespace FileService.Requests
{
    public class EditFileRequest
    {
        public string FileName { get; }
        public string Description { get; }

        public EditFileRequest(string fileName, string description)
        {
            FileName = fileName;
            Description = description;
        }
    }
}