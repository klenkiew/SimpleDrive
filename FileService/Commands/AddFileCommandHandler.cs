﻿using System;
using System.Linq;
using FileService.Cache;
using FileService.Database;
using FileService.Model;
using FileService.Queries;
using FileService.Services;

namespace FileService.Commands
{
    internal class AddFileCommandHandler : ICommandHandler<AddFileCommand>
    {
        private readonly FileDbContext fileDb;
        private readonly IFileStorage fileStorage;
        private readonly IUniversalCache cache;

        public AddFileCommandHandler(FileDbContext fileDb, IFileStorage fileStorage, IUniversalCache cache)
        {
            this.fileDb = fileDb;
            this.fileStorage = fileStorage;
            this.cache = cache;
        }

        public void Handle(AddFileCommand command)
        {
            var owner = fileDb.Users.FirstOrDefault(u => u.Id == command.Owner.Id);

            if (owner == null)
            {
                owner = command.Owner;
                fileDb.Users.Add(owner);
            }

            var file = new File()
            {
                FileName = command.FileName,
                Description = command.Description,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                PhysicalPath = "N/A",
                Size = command.Content.Length,
                OwnerId = command.Owner.Id
            };
            fileDb.Files.Add(file);
            fileDb.SaveChanges();

            fileStorage.SaveFile(owner, file.Id, command.Content);

            // invalidate the cache
            var query = new FindFilesByOwnerQuery()
            {
                OwnerId = owner.Id
            };
            cache.Remove(query);
        }
    }
}