using EventBus;

namespace FileService.Events
{
    public class FileUnsharedEvent : EventBase<FileSharesChangedMessage>
    {
        public FileUnsharedEvent(FileSharesChangedMessage message) : base(message)
        {}
    }
}