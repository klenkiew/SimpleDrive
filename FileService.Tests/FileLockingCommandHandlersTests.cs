using FileService.Commands;
using FileService.Events;
using FileService.Exceptions;
using FileService.Model;
using FileService.Tests.Fakes;
using NUnit.Framework;

namespace FileService.Tests
{
    public class FileLockingCommandHandlersTests
    {
        [Test]
        public void When_file_locked_by_its_owner_then_lock_owner_is_correct_and_proper_event_published()
        {
            // Arrange
            FakeFileLockingService fileLockingService = new FakeFileLockingService();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUserSource currentUserSource = new FakeCurrentUserSource();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file);
            currentUserSource.CurrentUser = file.Owner;

            var commandHandler = new AcquireFileLockCommandHandler(
                fileLockingService, fileRepository, eventPublisher, currentUserSource);
            
            var command = new AcquireFileLockCommand("fileId");
            
            // Act
            commandHandler.Handle(command);
            
            // Assert
            Assert.AreEqual(file.Owner.Id, fileLockingService.GetLockOwner(file).Id);
            
            FileLockChangedMessage publishedEvent = eventPublisher.VerifyPublishedOnce<FileLockChangedMessage>();
            Assert.AreEqual("fileId", publishedEvent.FileId);
        }
        
        [Test]
        public void File_can_be_locked_by_authorized_user()
        {
            // Arrange
            FakeFileLockingService fileLockingService = new FakeFileLockingService();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUserSource currentUserSource = new FakeCurrentUserSource();

            File file = new ExampleFileFactory().CreateFile();
            User otherUser = new User("someRandomUserId", "someRandomUser");
            file.ShareWith(otherUser);
            
            fileRepository.Save(file);
            currentUserSource.CurrentUser = otherUser;

            var commandHandler = new AcquireFileLockCommandHandler(
                fileLockingService, fileRepository, eventPublisher, currentUserSource);
            
            var command = new AcquireFileLockCommand("fileId");
            
            // Act
            commandHandler.Handle(command);
            
            // Assert
            Assert.AreEqual(otherUser.Id, fileLockingService.GetLockOwner(file).Id);

            FileLockChangedMessage publishedEvent = eventPublisher.VerifyPublishedOnce<FileLockChangedMessage>();

            Assert.AreEqual("fileId", publishedEvent.FileId);
            Assert.AreEqual(otherUser.Id, publishedEvent.NewLock.LockOwner.Id);
        }
        
        [Test]
        public void File_cannot_be_locked_by_not_authorized_user()
        {
            // Arrange
            FakeFileLockingService fileLockingService = new FakeFileLockingService();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUserSource currentUserSource = new FakeCurrentUserSource();

            File file = new ExampleFileFactory().CreateFile();
            User otherUser = new User("someRandomUserId", "someRandomUser");
            
            fileRepository.Save(file);
            currentUserSource.CurrentUser = otherUser;

            var commandHandler = new AcquireFileLockCommandHandler(
                fileLockingService, fileRepository, eventPublisher, currentUserSource);
            
            var command = new AcquireFileLockCommand("fileId");
            
            // Act
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void When_file_unlocked_then_lock_owner_is_null_and_proper_event_published()
        {
            // Arrange
            FakeFileLockingService fileLockingService = new FakeFileLockingService();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser = new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file);
            currentUser.Id = "currentUserId";
            fileLockingService.Lock(file, new User(currentUser.Id, currentUser.Username));
            
            var commandHandler = new RemoveFileLockCommandHandler(
                fileLockingService, fileRepository, currentUser, eventPublisher);
            
            var command = new RemoveFileLockCommand("fileId");
            
            // Act
            commandHandler.Handle(command);
            
            // Assert
            Assert.IsFalse(fileLockingService.IsLocked(file));
            Assert.IsNull(fileLockingService.GetLockOwner(file));
            
            var publishedEvent = eventPublisher.VerifyPublishedOnce<FileLockChangedMessage>();
            Assert.IsFalse(publishedEvent.NewLock.IsLockPresent);
        }
        
        [Test]
        public void File_cannot_be_unlocked_by_user_other_than_lock_owner()
        {
            // Arrange
            FakeFileLockingService fileLockingService = new FakeFileLockingService();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser = new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file);
            currentUser.Id = "currentUserId";
            fileLockingService.Lock(file, new User("someRandomUserId", "someRandomUser"));
            
            var commandHandler = new RemoveFileLockCommandHandler(
                fileLockingService, fileRepository, currentUser, eventPublisher);
            
            var command = new RemoveFileLockCommand("fileId");
            
            // Act
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void Not_locked_file_cannot_be_unlocked()
        {
            // Arrange
            FakeFileLockingService fileLockingService = new FakeFileLockingService();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser = new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file);
            currentUser.Id = "currentUserId";
            
            var commandHandler = new RemoveFileLockCommandHandler(
                fileLockingService, fileRepository, currentUser, eventPublisher);
            
            var command = new RemoveFileLockCommand("fileId");
            
            // Act
            Assert.Throws<NotFoundException>(() => commandHandler.Handle(command));
        }
    }
}