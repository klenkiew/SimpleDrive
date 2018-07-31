using System.Collections.Generic;

namespace FileService.Model
{
    public class User
    {
        public virtual string Id { get; private set; }
        public virtual string Username { get; private set; }
        
        public virtual ICollection<File> OwnedFiles { get; private set; }
        public virtual ICollection<FileShare> SharedFiles { get; private set; }

        private User()
        {
        }

        public User(string id, string username)
        {
            Id = id;
            Username = username;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        protected bool Equals(User other)
        {
            return string.Equals(Id, other.Id);
        }
    }
}