using System.Collections.Generic;
using EventBus;
using FileService.Infrastructure;

namespace FileService.Tests.Fakes
{
    public class FakeEventPublisher : IPostCommitEventPublisher, IPublisherWrapper 
    {
        public List<object> PublishedEvents { get; } = new List<object>();
        
        public void PublishAfterCommit<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>
        {
            Publish<TEvent, TMessage>(message);
        }

        public void Publish<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>
        {
            PublishedEvents.Add(message);
        }

        public FakeEventPublisher Verify()
        {
            return this;
        }
    }
}