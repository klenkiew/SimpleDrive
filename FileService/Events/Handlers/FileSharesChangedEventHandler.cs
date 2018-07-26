using Cache;
using EventBus;
using FileService.Queries;

namespace FileService.Events.Handlers
{
    public class FileSharesChangedEventHandler : IEventHandler<FileSharedEvent, FileSharesChangedMessage>, IEventHandler<FileUnsharedEvent, FileSharesChangedMessage>
    {
        private readonly IUniversalCache cache;

        public FileSharesChangedEventHandler(IUniversalCache cache)
        {
            this.cache = cache;
        }

        public void Handle(FileSharesChangedMessage fileInfo)
        {
            cache.Remove(new FindFilesByUserQuery(fileInfo.Share.UserId));
            cache.Remove(new FindUsersBySharedFileQuery(fileInfo.File.Id));
        }
    }
}