using EventBus;

namespace FileService.Events
{
    public class FileSharedEvent : EventBase<FileSharesChangedMessage>
    {
        public FileSharedEvent(FileSharesChangedMessage message) : base(message)
        {}
    }
}