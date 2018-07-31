using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FileService.Exceptions;

namespace FileService.Model
{
    public class File
    {
        public virtual string Id { get; private set; }
        public virtual string FileName { get; private set; }
        public virtual string Description { get; private set; }
        public virtual long Size { get; private set; }
        public virtual string PhysicalPath { get; private set; }
        public virtual string MimeType { get; private set; }
        public virtual DateTime DateCreated { get; private set; }
        public virtual DateTime DateModified { get; private set; }
        
        public virtual User Owner { get; private set; }
        
        public virtual ICollection<FileShare> SharedWith { get; private set; }

        private File() {}

        public File(string fileName, string description, long size, string mimeType, DateTime dateCreated, User owner)
        {
            FileName = fileName;
            Description = description;
            Size = size;
            PhysicalPath = "N/A";
            MimeType = mimeType;
            DateCreated = dateCreated;
            DateModified = dateCreated;
            Owner = owner;
            SharedWith = new List<FileShare>();
        }

        public bool IsOwnedBy(User user)
        {
            Ensure.NotNull(user, nameof(user));
            return user.Equals(Owner);
        }

        public bool CanBeModifiedBy(User user)
        {
            return Owner.Equals(user) || SharedWith.Any(sh => sh.UserId == user.Id);
        }
        
        public void Edit(string newFileName, string newDescription)
        {
            this.FileName = newFileName;
            this.Description = newDescription;
            this.DateModified = DateTime.UtcNow;
        }

        public void ContentChanged(DateTime when)
        {
            this.DateModified = when;
        }

        public void ShareWith(User shareWith)
        {
            if (Owner.Equals(shareWith))
                throw new PermissionException($"A file can't be shared with the owner.");
            
            var fileShare = new FileShare() {FileId = Id, UserId = shareWith.Id};
            SharedWith.Add(fileShare);
        }

        public void Unshare(User user)
        {
            FileShare shareToRemove = SharedWith.FirstOrDefault(fileShare => fileShare.User.Equals(user));

            if (shareToRemove == null)
                throw new NotFoundException($"Cannot unshare - a file with id {Id} isn't shared with this user.");

            SharedWith.Remove(shareToRemove);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((File) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        protected bool Equals(File other)
        {
            return string.Equals(Id, other.Id);
        }
    }
}