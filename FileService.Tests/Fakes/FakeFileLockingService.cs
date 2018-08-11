using System.Collections.Generic;
using FileService.Dto;
using FileService.Exceptions;
using FileService.Model;
using FileService.Services;

namespace FileService.Tests.Fakes
{
    public class FakeFileLockingService : IFileLockingService
    {
        private readonly Dictionary<File, User> fileToLockOwnerMap = new Dictionary<File, User>();
        
        public void Lock(File file, User lockedBy)
        {
            if (fileToLockOwnerMap.TryGetValue(file, out User lockOwner) && !Equals(lockOwner, lockedBy))
                throw new LockingException("The file is already locked");
            
            fileToLockOwnerMap.Add(file, lockedBy);
        }

        public void Unlock(File file, User lockedBy)
        {
            if (!fileToLockOwnerMap.TryGetValue(file, out User lockOwner))
                throw new LockingException("The file is not locked.");
            
            if (!lockedBy.Equals(lockOwner))
                throw new LockingException("The file can be unlocked only by the lock owner");

            fileToLockOwnerMap.Remove(file);
        }

        public UserDto GetLockOwner(File file)
        {
            fileToLockOwnerMap.TryGetValue(file, out User lockOwner);
            return lockOwner != null ? new UserDto(lockOwner.Id, lockOwner.Username) : null;
        }

        public bool IsLocked(File file)
        {
            return GetLockOwner(file) != null;
        }
    }
}