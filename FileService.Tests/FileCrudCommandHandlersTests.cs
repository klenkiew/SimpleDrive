using System.IO;
using System.Text;
using FileService.Commands;
using FileService.Exceptions;
using FileService.Model;
using FileService.Tests.Fakes;
using FileService.Tests.Helpers;
using NUnit.Framework;
using File = FileService.Model.File;

namespace FileService.Tests
{
    public class FileCrudCommandHandlersTests
    {
        [Test]
        public void File_edit_changes_filename_and_description_and_publishes_proper_event()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file, "fileId");
            currentUser.Id = file.Owner.Id;

            var commandHandler = new EditFileCommandHandler(fileRepository, currentUser, eventPublisher);
            
            var command = new EditFileCommand("fileId", "newFileName", "newFileDescription");
            
            // Act
            commandHandler.Handle(command);
            
            // Assert
            Assert.AreEqual("newFileName", file.FileName);
            Assert.AreEqual("newFileDescription", file.Description);
            
            File publishedEvent = eventPublisher.VerifyPublishedOnce<File>();
            Assert.AreEqual(file, publishedEvent);
        }
        
        [Test]
        public void File_can_only_be_edited_by_owner()
        {
            // Arrange
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();            
            fileRepository.Save(file, "fileId");
            currentUser.Id = "otherUserId";

            var commandHandler = new EditFileCommandHandler(fileRepository, currentUser, eventPublisher);
            
            var command = new EditFileCommand("fileId", "newFileName", "newFileDescription");
            
            // Assert & Act
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void File_remove_removes_file_and_publishes_event()
        {
            // Arrange
            FakeFileStorage fileStorage = new FakeFileStorage();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();
            EntityHelper.SetId(file, "fileId");
            fileRepository.Save(file, "fileId");
            fileStorage.SaveFile(file, Stream.Null);
            currentUser.Id = file.Owner.Id;

            var commandHandler = new DeleteFileCommandHandler(fileStorage, currentUser, fileRepository, eventPublisher);
            
            var command = new DeleteFileCommand("fileId");
            
            // Act
            commandHandler.Handle(command);
            
            // Assert
            Assert.IsNull(fileRepository.GetById("fileId"));
            Assert.False(fileStorage.Exists(file));
            
            File publishedEvent = eventPublisher.VerifyPublishedOnce<File>();
            Assert.AreEqual(file, publishedEvent);
        }
        
        [Test]
        public void File_can_be_removed_only_by_owner()
        {
            // Arrange
            FakeFileStorage fileStorage = new FakeFileStorage();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUser currentUser= new FakeCurrentUser();

            File file = new ExampleFileFactory().CreateFile();
            EntityHelper.SetId(file, "fileId");
            fileRepository.Save(file, "fileId");
            fileStorage.SaveFile(file, Stream.Null);
            currentUser.Id = "otherUserId";

            var commandHandler = new DeleteFileCommandHandler(fileStorage, currentUser, fileRepository, eventPublisher);
            
            var command = new DeleteFileCommand("fileId");
            
            // Assert & Act
            Assert.Throws<PermissionException>(() => commandHandler.Handle(command));
        }
        
        [Test]
        public void File_add_adds_file_and_publishes_event()
        {
            // Arrange
            FakeFileStorage fileStorage = new FakeFileStorage();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUserSource currentUserSource = new FakeCurrentUserSource();
            FakePostCommitRegistrar postCommitRegistrar = new FakePostCommitRegistrar();

            currentUserSource.CurrentUser = new User("userId", "username");

            var commandHandler = new AddFileCommandHandler(
                fileRepository, fileStorage, eventPublisher, postCommitRegistrar, currentUserSource);

            var contentBuffer = Encoding.UTF8.GetBytes("testContent");
            
            using (Stream fileContent = new MemoryStream(contentBuffer))
            {
                var command = new AddFileCommand("filename", "desc", "text/plain", fileContent);

                // Act
                commandHandler.Handle(command);
                postCommitRegistrar.ExecuteActions();

                // Assert
                File addedFile = fileRepository.GetByName("filename");
                
                Assert.IsNotNull(addedFile);
                Assert.AreEqual("filename", addedFile.FileName);
                Assert.AreEqual("desc", addedFile.Description);
                Assert.AreEqual("userId", addedFile.Owner.Id);

                Stream fileStream = fileStorage.ReadFile(addedFile).Result;
                Assert.IsNotNull(fileStream);

                using (var reader = new StreamReader(fileStream))
                    Assert.AreEqual("testContent", reader.ReadToEnd());

                File publishedEvent = eventPublisher.VerifyPublishedOnce<File>();
                Assert.AreEqual(addedFile, publishedEvent);
            }
        }

        [Test]
        public void File_add_throws_when_owner_now_found()
        {
            // Arrange
            FakeFileStorage fileStorage = new FakeFileStorage();
            FakeFileRepository fileRepository = new FakeFileRepository();
            FakeEventPublisher eventPublisher = new FakeEventPublisher();
            FakeCurrentUserSource currentUserSource = new FakeCurrentUserSource();
            FakePostCommitRegistrar postCommitRegistrar = new FakePostCommitRegistrar();

            currentUserSource.CurrentUser = null;

            var commandHandler = new AddFileCommandHandler(
                fileRepository, fileStorage, eventPublisher, postCommitRegistrar, currentUserSource);

            var contentBuffer = Encoding.UTF8.GetBytes("testContent");

            using (Stream fileContent = new MemoryStream(contentBuffer))
            {
                var command = new AddFileCommand("filename", "desc", "text/plain", fileContent);

                // Assert & Act
                Assert.Throws<NotFoundException>(() =>
                {
                    commandHandler.Handle(command);
                    postCommitRegistrar.ExecuteActions();
                });
            }
        }
    }
}