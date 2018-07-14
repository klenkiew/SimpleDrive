﻿using System;
using Cache;
using FileService.Configuration;
using FileService.Dto;
using FileService.Exceptions;
using FileService.Model;

namespace FileService.Services
{
    public class FileLockingService : IFileLockingService
    {
        private readonly ICache lockCache;
        private readonly ICurrentUser currentUser;
        private readonly TimeSpan fileLockDuration;

        public FileLockingService(ICache lockCache, ICurrentUser currentUser, StorageConfiguration configuration)
        {
            this.lockCache = lockCache;
            this.currentUser = currentUser;
            this.fileLockDuration = configuration.FileLockDuration;
        }

        public void Lock(File file)
        {
            var currentUserDto = new UserDto(currentUser.Id, currentUser.Username, "N/A");
            var owner = lockCache.ComputeIfAbsent(file.Id, () => currentUserDto, fileLockDuration);
            
            if (owner != null)
            {
                if (owner.Id != currentUserDto.Id)
                    throw new LockingException("The file is already locked.");
                // ComputeIfAbsent doesn't refresh a cache entry if the value is already present
                // so the Set method must be invoked to reset the timer for this cache entry
                lockCache.Set(file.Id, currentUserDto);
            }
        }

        public void Unlock(File file)
        {
            var lockingUser = lockCache.Get<UserDto>(file.Id);
            if (lockingUser == null)
                throw new LockingException("The file cannot bu unlocked because it is not locked.");
            
            if (lockingUser.Id != currentUser.Id)
                throw new LockingException("The file cannot bu unlocked because the user is not the lock owner.");
            
            lockCache.Remove(file.Id);
        }

        public UserDto GetLockOwner(File file)
        {
            return lockCache.Get<UserDto>(file.Id);
        }
        
        public bool IsLocked(File file)
        {
            return lockCache.Get<UserDto>(file.Id) != null;
        }
    }
}