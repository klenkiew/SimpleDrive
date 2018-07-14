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
        public string FileId { get; set; }
        public FileLockDto NewLock { get; set; } 
    }
}