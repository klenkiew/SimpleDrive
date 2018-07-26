using EventBus;
using FileService.Model;

namespace FileService.Events
{
    public class FileSharedEvent : EventBase<File>
    {
        public FileSharedEvent(File message) : base(message)
        {}
    }
}