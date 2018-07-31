using Cache;
using EventBus;
using FileService.Model;
using FileService.Queries;

namespace FileService.Events.Handlers
{
    public class FileAddedEventHandler : IEventHandler<FileAddedEvent, File>
    {
        private readonly IUniversalCache cache;

        public FileAddedEventHandler(IUniversalCache cache)
        {
            this.cache = cache;
        }

        public void Handle(File file)
        {
            cache.Remove(new FindFilesByUserQuery(file.Owner.Id));
        }
    }
}