using FileService.Dto;
using FileService.Exceptions;
using FileService.Model;

namespace FileService.Services
{
    public interface IFileLockingService
    {
        void Lock(File file, User lockedBy);
        void Unlock(File file, User lockedBy);
        UserDto GetLockOwner(File file);
        bool IsLocked(File file);
    }

    public static class FileLockingServiceExtensions
    {
        public static FileLock CreateLock(this IFileLockingService fileLockingService, File fileToLock, User lockOwner)
        {
            return new FileLock(fileToLock, fileLockingService, lockOwner);
        }

        public static UserDto GetRequiredLockOwner(this IFileLockingService fileLockingService, File file)
        {
            return fileLockingService.GetLockOwner(file) 
                   ?? throw new NotFoundException($"The file with id {file.Id} is not locked.");;
        }
    }
}