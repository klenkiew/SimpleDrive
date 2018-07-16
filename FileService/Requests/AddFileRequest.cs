using System;
using Microsoft.AspNetCore.Http;

namespace FileService.Requests
{
    public class AddFileRequest
    {
        // the setters and parameterless constructor are required by the model binder
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }

        public AddFileRequest(IFormFile file, string fileName, string description)
        {
            File = file;
            FileName = fileName;
            Description = description;
        }

        [Obsolete("Parameterless constructor only for model binding purposes.", true)]
        public AddFileRequest()
        {
        }
    }
}