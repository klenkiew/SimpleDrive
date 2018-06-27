using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FileService.Model
{
    public class User
    {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        
        public virtual ICollection<File> OwnedFiles { get; set; }
    }
}