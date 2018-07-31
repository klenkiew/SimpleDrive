using System.Linq;
using Cache;
using EventBus;
using FileService.Model;
using FileService.Queries;

namespace FileService.Events.Handlers
{
    public class FileEditedEventHandler : IEventHandler<FileDeletedEvent, File>, IEventHandler<FileEditedEvent, File>
    {
        private readonly IUniversalCache cache;

        public FileEditedEventHandler(IUniversalCache cache)
        {
            this.cache = cache;
        }

        public void Handle(File file)
        {
            cache.Remove(new FindFilesByUserQuery(file.Owner.Id));
            cache.Remove(new FindFileByIdQuery(file.Id));
            cache.Remove(new FindUsersBySharedFileQuery(file.Id));
            
            foreach (string userId in file.SharedWith.Select(shared => shared.UserId))
                cache.Remove(new FindFilesByUserQuery(userId));
        }
    }
}