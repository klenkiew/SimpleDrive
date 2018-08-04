using System;
using FileService.Model;

namespace FileService.Services
{
    public class FileLock : IDisposable
    {
        private readonly File lockedFile;
        private readonly User lockOwner;
        private readonly IFileLockingService fileLockingService;

        public FileLock(File fileToLock, IFileLockingService fileLockingService, User lockOwner)
        {
            this.lockedFile = fileToLock;
            this.fileLockingService = fileLockingService;
            this.lockOwner = lockOwner;
            this.fileLockingService.Lock(fileToLock, lockOwner);
        }

        public void Dispose()
        {
            fileLockingService.Unlock(lockedFile, lockOwner);
        }
    }
}