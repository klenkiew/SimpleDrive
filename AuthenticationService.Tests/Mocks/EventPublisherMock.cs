using System;
using System.Linq.Expressions;
using EventBus;
using Moq;

namespace AuthenticationService.Tests.Mocks
{
    public class EventPublisherMock : Mock<IPublisherWrapper>
    {
        public void VerifyEventPublished<TEvent, TMessage>(Expression<Func<TMessage, bool>> predicate) where TEvent : IEvent<TMessage>
        {
            Verify(publisher => publisher.Publish<TEvent, TMessage>(It.Is(predicate)));
        }
    }
}