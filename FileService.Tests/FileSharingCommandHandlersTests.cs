using FileService.Commands;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Tests.Fakes;
using NUnit.Framework;

namespace FileService.Tests
{
    public class FileSharingCommandHandlersTests
    {
        [Test]
        public void File_can_be_shared_and_allows_for_modifications_by_authorized_user()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeUserRepository userRepository = new FakeUserRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            User shareWith = new User("shareWithUserId", "username");
            fileRepository.Save(file, "fileId");
            userRepository.Save(shareWith, shareWith.Id);
            currentUser.Id = file.Owner.Id;

            var commandHandler = new ShareFileCommandHandler(
                currentUser, fileRepository, userRepository, eventPublisher);
            
            var command = new ShareFileCommand("fileId", shareWith.Id);
            
            // Act
            commandHandler.Handle(command);
            
            // Assert
            Assert.IsTrue(file.CanBeModifiedBy(shareWith));
            FileSharesChangedMessage publishedEvent = eventPublisher.VerifyPublishedOnce<FileSharesChangedMessage>();
            Assert.AreEqual(file, publishedEvent.File);
        }
        
        [Test]
        public void File_cannot_be_shared_with_non_existing_user()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeUserRepository userRepository = new FakeUserRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser = new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            User shareWith = new User("shareWithUserId", "username");
            fileRepository.Save(file, "fileId");
            currentUser.Id = file.Owner.Id;

            var commandHandler = new ShareFileCommandHandler(
                currentUser, fileRepository, userRepository, eventPublisher);
            
            var command = new ShareFileCommand("fileId", shareWith.Id);
            
            // Act & Assert
            Assert.Throws<NotFoundException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void File_cannot_be_shared_by_random_user()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeUserRepository userRepository = new FakeUserRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            User shareWith = new User("shareWithUserId", "username");
            fileRepository.Save(file, "fileId");
            userRepository.Save(shareWith, shareWith.Id);
            currentUser.Id = shareWith.Id;

            var commandHandler = new ShareFileCommandHandler(
                currentUser, fileRepository, userRepository, eventPublisher);
            
            var command = new ShareFileCommand("fileId", shareWith.Id);
            
            // Act & Assert
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void File_cannot_be_shared_by_user_which_has_access_but_is_not_owner()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeUserRepository userRepository = new FakeUserRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            User shareWith = new User("shareWithUserId", "username");
            fileRepository.Save(file, "fileId");
            userRepository.Save(shareWith, shareWith.Id);
            file.ShareWith(shareWith);
            currentUser.Id = shareWith.Id;

            var commandHandler = new ShareFileCommandHandler(
                currentUser, fileRepository, userRepository, eventPublisher);
            
            var command = new ShareFileCommand("fileId", shareWith.Id);
            
            // Act & Assert
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void File_cannot_be_shared_with_owner()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeUserRepository userRepository = new FakeUserRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file, "fileId");
            userRepository.Save(file.Owner, file.Owner.Id);
            currentUser.Id = file.Owner.Id;

            var commandHandler = new ShareFileCommandHandler(
                currentUser, fileRepository, userRepository, eventPublisher);
            
            var command = new ShareFileCommand("fileId", file.Owner.Id);
            
            // Act & Assert
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
    }
}