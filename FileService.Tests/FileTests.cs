using System;
using System.Globalization;
using FileService.Exceptions;
using FileService.Model;
using NUnit.Framework;

namespace FileService.Tests
{
    public class FileTests
    {
        [Test]
        public void File_IsOwnedBy_returns_true_for_owner()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();

            // Act
            bool isOwnedBy = file.IsOwnedBy(file.Owner);
            
            // Assert
            Assert.True(isOwnedBy);
        }
        
        [Test]
        public void File_IsOwnedBy_returns_true_for_user_with_same_id_as_owner()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            User fileOwner = file.Owner;
            var user = new User(fileOwner.Id, fileOwner.Username);

            // Act
            bool isOwnedBy = file.IsOwnedBy(user);

            // Assert
            Assert.True(isOwnedBy);
        }
        
        [Test]
        public void File_IsOwnedBy_returns_false_for_user_with_different_id_than_owner()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            var notOwner = new User("f10b5f27-5470-49aa-af83-754dfffa7c2c", "otherUser");

            // Act
            bool isOwnedBy = file.IsOwnedBy(notOwner);

            // Assert
            Assert.False(isOwnedBy);
        }
        
        [Test]
        public void When_edited_file_name_and_description_change()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            const string newFileName = "otherName";
            const string newDescription = "otherDescription";
            
            // Act
            file.Edit(newFileName, newDescription, file.DateCreated+TimeSpan.FromHours(1));
            
            // Assert
            Assert.AreEqual(newFileName, file.FileName);
            Assert.AreEqual(newDescription, file.Description);
        }
        
        [Test]
        public void When_edited_file_modification_date_changes()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            const string newFileName = "otherName";
            const string newDescription = "otherDescription";
            DateTime dateModified = file.DateCreated+TimeSpan.FromHours(1);

            // Act
            file.Edit(newFileName, newDescription, dateModified);
            
            // Assert
            Assert.AreEqual(dateModified, file.DateModified);
        }
        
        [Test]
        public void When_content_changed_file_modification_date_changes()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            DateTime dateModified = file.DateCreated+TimeSpan.FromHours(1);

            // Act
            file.ContentChanged(dateModified);
            
            // Assert
            Assert.AreEqual(dateModified, file.DateModified);
        }
        
        [Test]
        public void File_can_be_modified_by_owner()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            User fileOwner = file.Owner;

            // Act
            bool canBeModifiedByOwner = file.CanBeModifiedBy(fileOwner);
            
            // Assert
            Assert.True(canBeModifiedByOwner);
        }
        
        [Test]
        public void File_cannnot_be_modified_by_random_user()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            var someRandomUser = new User("f10b5f27-5470-49aa-af83-754dfffa7c2c", "otherUser");

            // Act
            bool canBeModifiedBySomeRandom = file.CanBeModifiedBy(someRandomUser);
            
            // Assert
            Assert.False(canBeModifiedBySomeRandom);
        }
        
        [Test]
        public void File_can_be_modified_by_authorized_user()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            var someRandomUser = new User("f10b5f27-5470-49aa-af83-754dfffa7c2c", "otherUser");
            file.ShareWith(someRandomUser);
            
            // Act
            bool canBeModifiedBySomeRandom = file.CanBeModifiedBy(someRandomUser);
            
            // Assert
            Assert.True(canBeModifiedBySomeRandom);
        }
        
        [Test]
        public void File_cannot_be_modified_after_unsharing()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            var someRandomUser = new User("f10b5f27-5470-49aa-af83-754dfffa7c2c", "otherUser");
            file.ShareWith(someRandomUser);
            file.Unshare(someRandomUser);
            
            // Act
            bool canBeModifiedBySomeRandom = file.CanBeModifiedBy(someRandomUser);
            
            // Assert
            Assert.False(canBeModifiedBySomeRandom);
        }
        
        [Test]
        public void File_cannot_be_shared_with_its_owner()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            
            //Assert
            Assert.Throws<PermissionException>(() => file.ShareWith(file.Owner));
        }
        
        [Test]
        public void File_cannot_be_unshared_when_not_shared()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            var someRandomUser = new User("f10b5f27-5470-49aa-af83-754dfffa7c2c", "otherUser");
            
            //Assert
            Assert.Throws<NotFoundException>(() => file.Unshare(someRandomUser));
        }
        
        [Test]
        public void File_cannot_be_unshared_with_its_owner()
        {
            // Arrange 
            File file = new ExampleFileFactory().CreateFile();
            
            //Assert
            Assert.Throws<NotFoundException>(() => file.Unshare(file.Owner));
        }
    }
}