using EventBus;

namespace FileService.Infrastructure
{
    public interface IPostCommitEventPublisher
    {
        void PublishAfterCommit<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>;
    }
}