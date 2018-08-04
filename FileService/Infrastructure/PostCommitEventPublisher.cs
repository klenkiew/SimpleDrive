using EventBus;
using FileService.Database;

namespace FileService.Infrastructure
{
    public class PostCommitEventPublisher : IPostCommitEventPublisher
    {
        private readonly IPublisherWrapper eventBus;
        private readonly IPostCommitRegistrator registrator;

        public PostCommitEventPublisher(IPublisherWrapper eventBus, IPostCommitRegistrator registrator)
        {
            this.eventBus = eventBus;
            this.registrator = registrator;
        }

        public void PublishAfterCommit<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>
        {
            registrator.Committed += () => eventBus.Publish<TEvent, TMessage>(message);
        }
    }
}