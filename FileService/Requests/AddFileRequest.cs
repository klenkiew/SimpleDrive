using Microsoft.AspNetCore.Http;

namespace FileService.Requests
{
    public class AddFileRequest
    {
        public IFormFile File { get; }
        public string FileName { get; }
        public string Description { get; }

        public AddFileRequest(IFormFile file, string fileName, string description)
        {
            File = file;
            FileName = fileName;
            Description = description;
        }
    }
}