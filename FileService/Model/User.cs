using System.Collections.Generic;

namespace FileService.Model
{
    public class User
    {
        public virtual string Id { get; set; }
        public virtual string Username { get; set; }
        
        public virtual ICollection<File> OwnedFiles { get; set; }
        public virtual ICollection<FileShare> SharedFiles { get; set; }
    }
}