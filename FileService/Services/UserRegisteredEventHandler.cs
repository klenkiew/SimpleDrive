using System;
using CommonEvents;
using EventBus;
using FileService.Database;
using FileService.Model;
using Microsoft.Extensions.Logging;

namespace FileService.Services
{
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent, UserInfo>, IDisposable
    {
        private readonly FileDbContext fileDb;
        private readonly ILogger<UserRegisteredEventHandler> logger;

        public UserRegisteredEventHandler(FileDbContext fileDb, ILoggerFactory loggerFactory)
        {
            this.fileDb = fileDb ?? throw new ArgumentNullException(nameof(fileDb));
            this.logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)))
                .CreateLogger<UserRegisteredEventHandler>();
        }

        public void Handle(UserInfo message)
        {
            // TODO logging decorator for message handlers? or adapter to reuse command handlers decorators
            logger.LogTrace("User added: " + message.Username);
            fileDb.Users.Add(new User() {Id = message.Id, Username = message.Username});
            fileDb.SaveChanges();
        }

        public void Dispose()
        {
            fileDb?.Dispose();
        }
    }
}