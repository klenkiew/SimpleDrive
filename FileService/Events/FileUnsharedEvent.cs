using EventBus;
using FileService.Model;

namespace FileService.Events
{
    public class FileUnsharedEvent : EventBase<File>
    {
        public FileUnsharedEvent(File message) : base(message)
        {}
    }
}