using Microsoft.AspNetCore.StaticFiles;

namespace FileService.Services
{
    internal class MimeTypeMap : IMimeTypeMap
    {
        private readonly FileExtensionContentTypeProvider fileExtensionContentTypeProvider 
            = new FileExtensionContentTypeProvider();

        public string GetMimeType(string filename)
        {
            fileExtensionContentTypeProvider.TryGetContentType(filename, out var contentType);
            return contentType;
        }
    }
}