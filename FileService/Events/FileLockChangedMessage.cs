using EventBus;
using FileService.Dto;

namespace FileService.Events
{
    public class FileLockChangedEvent : EventBase<FileLockChangedMessage>
    {
        public FileLockChangedEvent(FileLockChangedMessage message) : base(message)
        {
        }
    }

    public class FileLockChangedMessage
    {
        public string FileId { get; }
        public FileLockDto NewLock { get; }

        public FileLockChangedMessage(string fileId, FileLockDto newLock)
        {
            FileId = fileId;
            NewLock = newLock;
        }
    }
}