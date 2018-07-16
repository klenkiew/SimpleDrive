using System;
using FileService.Model;

namespace FileService.Dto
{
    public class FileDto
    {
        public FileDto(string id, string fileName, string description, long size, string mimeType, 
            DateTime dateCreated, UserDto owner)
        {
            Id = id;
            FileName = fileName;
            Description = description;
            Size = size;
            MimeType = mimeType;
            DateCreated = dateCreated;
            Owner = owner;
        }

        public string Id { get; }
        public string FileName { get; }
        public string Description { get; }
        public long Size { get; }
        public string MimeType { get; }
        public DateTime DateCreated { get; }
          
        public UserDto Owner { get; }
    }
}