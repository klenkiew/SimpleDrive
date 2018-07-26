using EventBus;
using FileService.Model;

namespace FileService.Events
{
    public class FileAddedEvent : EventBase<File>
    {
        public FileAddedEvent(File message) : base(message)
        {}
    }
}