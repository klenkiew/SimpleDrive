using System;

namespace FileService.Services
{
    public interface IFileLockExpiryNotificator
    {
        void ResetFileLock(string fileId, TimeSpan fileLockDuration);
        void RemoveFileLock(string fileId);
    }
}