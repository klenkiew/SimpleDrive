using System;
using System.Collections.Generic;

namespace FileService.Model
{
    public class File
    {
        public virtual string Id { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Description { get; set; }
        public virtual long Size { get; set; }
        public virtual string PhysicalPath { get; set; }
        public virtual string MimeType { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        
        public virtual string OwnerId { get; set; }
        public virtual string OwnerName { get; set; }
        public virtual User Owner { get; set; }
        
        public virtual ICollection<FileShare> SharedWith{ get; set; }
    }
}