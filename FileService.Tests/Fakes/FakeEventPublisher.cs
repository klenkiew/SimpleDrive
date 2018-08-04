using System.Collections.Generic;
using EventBus;
using FileService.Infrastructure;
using NUnit.Framework;

namespace FileService.Tests.Fakes
{
    public class FakeEventPublisher : IPostCommitEventPublisher, IPublisherWrapper 
    {
        private List<object> PublishedEvents { get; } = new List<object>();
        
        public void PublishAfterCommit<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>
        {
            Publish<TEvent, TMessage>(message);
        }

        public void Publish<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>
        {
            PublishedEvents.Add(message);
        }

        public TMessage VerifyPublishedOnce<TMessage>()
        {
            Assert.AreEqual(1, PublishedEvents.Count);
            Assert.IsInstanceOf<TMessage>(PublishedEvents[0]);
            return (TMessage) PublishedEvents[0];
        }
    }
}