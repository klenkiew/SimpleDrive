using FileService.Dto;
using FileService.Model;

namespace FileService.Services
{
    public interface IFileLockingService
    {
        void Lock(File file);
        void Unlock(File file);
        UserDto GetLockOwner(File file);
        bool IsLocked(File file);
    }

    public static class FileLockingServiceExtensions
    {
        public static FileLock CreateLock(this IFileLockingService fileLockingService, File fileToLock)
        {
            return new FileLock(fileToLock, fileLockingService);
        }
    }
}