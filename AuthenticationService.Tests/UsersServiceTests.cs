using System.Collections.Generic;
using System.Linq;
using AuthenticationService.Database;
using AuthenticationService.Dto;
using AuthenticationService.Model;
using AuthenticationService.Services;
using NUnit.Framework;

namespace AuthenticationService.Tests
{
    public class UsersServiceTests
    {
        [Test]
        public void Get_all_users_returns_empty_enumerable_when_database_is_empty()
        {
            // Arrange
            using (UserDbContext context = InMemoryDatabase.Get())
            {
                var service = new UsersService(context);
                
                // Act
                IEnumerable<UserDto> users = service.GetAllUsers().Result;
                
                // Assert
                Assert.IsEmpty(users);
            }
        }
        
        [Test]
        public void Get_all_users_returns_proper_users_from_non_empty_database()
        {
            // Arrange
            using (UserDbContext context = InMemoryDatabase.Get())
            {
                context.Users.Add(new User() {Id = "user1"});
                context.Users.Add(new User() {Id = "user2"});
                context.SaveChanges();
            }

            using (UserDbContext context = InMemoryDatabase.Get())
            {
                var service = new UsersService(context);

                // Act
                List<UserDto> users = service.GetAllUsers().Result.ToList();
                
                // Assert
                Assert.AreEqual(2, users.Count);
                Assert.AreEqual("user1", users[0].Id);
                Assert.AreEqual("user2", users[1].Id);
            }            
        }
        
        [Test]
        public void Get_users_by_prefix_returns_matching_users()
        {
            // Arrange
            using (UserDbContext context = InMemoryDatabase.Get())
            {
                context.Users.Add(new User() {Id = "1", Username = "user1"});
                context.Users.Add(new User() {Id = "2", Username = "User2"});
                context.Users.Add(new User() {Id = "3", Username = "Auser2"});
                context.Users.Add(new User() {Id = "4", Username = "user"});
                context.SaveChanges();
            }

            using (UserDbContext context = InMemoryDatabase.Get())
            {
                var service = new UsersService(context);

                // Act
                List<UserDto> users = service.GetUsersByNamePrefix("user").Result.OrderBy(u => u.Id).ToList();
                
                // Assert
                Assert.AreEqual(3, users.Count);
                Assert.AreEqual("user1", users[0].Username);
                Assert.AreEqual("User2", users[1].Username);
                Assert.AreEqual("user", users[2].Username);
            }            
        }

        [TearDown]
        public void ClearDatabase()
        {
            InMemoryDatabase.Get().Database.EnsureDeleted();
        }
    }
}