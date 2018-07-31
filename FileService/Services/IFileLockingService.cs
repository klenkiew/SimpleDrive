using FileService.Dto;
using FileService.Exceptions;
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

        public static UserDto GetRequiredLockOwner(this IFileLockingService fileLockingService, File file)
        {
            return fileLockingService.GetLockOwner(file) 
                   ?? throw new NotFoundException($"The file with id {file.Id} is not locked.");;
        }
    }
}