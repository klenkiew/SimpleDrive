using EventBus;
using FileService.Model;

namespace FileService.Events
{
    public class FileDeletedEvent : EventBase<File>
    {
        public FileDeletedEvent(File message) : base(message)
        {}
    }
}