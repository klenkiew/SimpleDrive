using System;
using FileService.Model;

namespace FileService.Services
{
    public class FileLock : IDisposable
    {
        private readonly File lockedFile;
        private readonly IFileLockingService fileLockingService;

        public FileLock(File fileToLock, IFileLockingService fileLockingService)
        {
            this.lockedFile = fileToLock;
            this.fileLockingService = fileLockingService;
            this.fileLockingService.Lock(fileToLock);
        }

        public void Dispose()
        {
            fileLockingService.Unlock(lockedFile);
        }
    }
}