using System;

namespace EventBus
{
    public interface IEventBusWrapper
    {
        void Publish<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>;
        void Subscribe<TEvent, TMessage>(IEventHandler<TEvent, TMessage> eventHandler) where TEvent : IEvent<TMessage>;
    }

    public static class EventBusWrapperExtensions
    {
        public static void Subscribe<TEvent, TMessage>(this IEventBusWrapper @this, 
            Action<TMessage> messageHandler) where TEvent : IEvent<TMessage>
        {
            @this.Subscribe(new EventHandlerAdapter<TEvent, TMessage>(messageHandler));
        }
        
    }
    
    internal class EventHandlerAdapter<TEvent, TMessage> : IEventHandler<TEvent, TMessage> where TEvent : IEvent<TMessage>
    {
        private readonly Action<TMessage> handler;

        public EventHandlerAdapter(Action<TMessage> handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Handle(TMessage message)
        {
            handler.Invoke(message);
        }
    }
}