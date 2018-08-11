using System;

namespace FileService.Dto
{
    public class FileDto
    {
        public string Id { get; }
        public string FileName { get; }
        public string Description { get; }
        public long Size { get; }
        public string MimeType { get; }
        public DateTime DateCreated { get; }
        public DateTime DateModified { get; }
          
        public UserDto Owner { get; set; }

        public FileDto(string id, string fileName, string description, long size, string mimeType, 
            DateTime dateCreated, DateTime dateModified, UserDto owner)
        {
            Id = id;
            FileName = fileName;
            Description = description;
            Size = size;
            MimeType = mimeType;
            DateCreated = dateCreated;
            DateModified = dateModified;
            Owner = owner;
        }
        
        private FileDto(string id, string fileName, string description, long size, string mimeType, 
            DateTime dateCreated, DateTime dateModified)
        {
            Id = id;
            FileName = fileName;
            Description = description;
            Size = size;
            MimeType = mimeType;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }
    }
}