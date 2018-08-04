using FileService.Model;
using NUnit.Framework;

namespace FileService.Tests
{
    public class UserTests
    {
        [Test]
        public void User_is_equal_when_compared_with_same_instance()
        {
            var user = new User("id", "username");
            
            // ReSharper disable once EqualExpressionComparison
            Assert.True(user.Equals(user));
        }
        
        [Test]
        public void User_is_equal_when_compared_to_different_instance_with_same_id()
        {
            var user = new User("id", "username");
            var sameUser = new User("id", "username");
            
            Assert.True(user.Equals(sameUser));
        }
        
        [Test]
        public void User_is_not_equal_when_compared_to_user_with_different_id()
        {
            var user = new User("id", "username");
            var differentUser = new User("differentId", "username");
            
            Assert.False(user.Equals(differentUser));
        }
    }
}