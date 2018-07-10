using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using CommonEvents;
using EventBus;
using FileService.Database;
using FileService.Dto;
using FileService.Infrastructure.HttpClient;
using FileService.Infrastructure.ScopedServices;
using FileService.Model;
using Microsoft.Extensions.Logging;

namespace FileService.Infrastructure
{
    public class UsersIntegrationService : IUsersIntegrationService
    {
        private readonly IScopedServiceFactory<FileDbContext> dbContextScopeFactory;
        private readonly IHttpClientWrapper httpClientWrapper;
        private readonly IEventBusWrapper eventBus;
        private readonly ILogger<UsersIntegrationService> logger;
        
        public UsersIntegrationService(
            IScopedServiceFactory<FileDbContext> dbContextScopeFactory, 
            IHttpClientWrapper httpClientWrapper, 
            IEventBusWrapper eventBus,
            ILoggerFactory loggerFactory)
        {
            this.dbContextScopeFactory = dbContextScopeFactory;
            this.httpClientWrapper = httpClientWrapper;
            this.eventBus = eventBus;
            this.logger = loggerFactory.CreateLogger<UsersIntegrationService>();
        }

        public void Run()
        {
            try
            {
                FetchExistingUsers();
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException)
            {
                logger.LogInformation(ex, "Failed to connect to the Authentication Service. " +
                                          "Subscribing to the OnAuthenticationServiceStarted event.");
                eventBus.Subscribe<IEvent<AuthenticationServiceStarted>, AuthenticationServiceStarted>(started => FetchExistingUsers());   
            }
        }

        private void FetchExistingUsers()
        {
            logger.LogInformation("Fetching existing users from the Authentication Service");
            var users = httpClientWrapper.Get<IEnumerable<UserDto>>("http://localhost:5000/api/Users/GetAllUsers");
            logger.LogInformation("Existing users fetched successfully");
            using (var scope = this.dbContextScopeFactory.CreateScope())
            {
                var dbContext = scope.GetService();
                const string ignoredFieldsValue = "ignore";
                var dbUsers = dbContext.Users.Select(u => new UserDto(u.Id, ignoredFieldsValue, ignoredFieldsValue)).ToList();
                var existingUsers = new HashSet<UserDto>(dbUsers, new UserDtoEqualityComparer());
                int newUsersCount = 0;
                foreach (var userDto in users)
                {
                    if (existingUsers.Contains(userDto))
                        continue;
                    dbContext.Users.Add(new User() {Id = userDto.Id, Username = userDto.Username});
                    ++newUsersCount;
                }

                logger.LogInformation($"New users found: {newUsersCount}");
                dbContext.SaveChanges();
                logger.LogInformation("The new users successfully added to the database");    
            }
        }
    }
    
    
}