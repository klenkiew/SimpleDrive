using System;
using CommonEvents;
using EventBus;
using FileService.Database;
using FileService.Model;

namespace FileService.Events.Handlers
{
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent, UserInfo>, IDisposable
    {
        private readonly FileDbContext fileDb;

        public UserRegisteredEventHandler(FileDbContext fileDb)
        {
            this.fileDb = fileDb ?? throw new ArgumentNullException(nameof(fileDb));
        }

        public void Handle(UserInfo userInfo)
        {
            fileDb.Users.Add(new User(userInfo.Id, userInfo.Username));
            fileDb.SaveChanges();
        }

        public void Dispose()
        {
            fileDb?.Dispose();
        }
    }
}