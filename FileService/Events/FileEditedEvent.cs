using EventBus;
using FileService.Model;

namespace FileService.Events
{
    public class FileEditedEvent : EventBase<File>
    {
        public FileEditedEvent(File message) : base(message)
        {}
    }
}