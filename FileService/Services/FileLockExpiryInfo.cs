using System;

namespace FileService.Services
{
    internal class FileLockExpiryInfo
    {
        public string FileId { get; }
        public DateTime ExpiryDate { get; }

        public FileLockExpiryInfo(string fileId, DateTime expiryDate)
        {
            FileId = fileId;
            ExpiryDate = expiryDate;
        }

        private bool Equals(FileLockExpiryInfo other)
        {
            return string.Equals(FileId, other.FileId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileLockExpiryInfo) obj);
        }

        public override int GetHashCode()
        {
            return (FileId != null ? FileId.GetHashCode() : 0);
        }
    }
}