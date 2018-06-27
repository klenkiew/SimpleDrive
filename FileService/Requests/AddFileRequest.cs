using Microsoft.AspNetCore.Http;

namespace FileService.Requests
{
    public class AddFileRequest
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}