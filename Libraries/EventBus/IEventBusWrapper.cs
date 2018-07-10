using System;

namespace EventBus
{
    public interface IEventBusWrapper
    {
        void Publish<TEvent, TMessage>(TEvent @event) where TEvent : IEvent<TMessage>;
        void Subscribe<TEvent, TMessage>(IMessageHandler<TMessage> messageHandler) where TEvent : IEvent<TMessage>;
    }

    public static class EventBusWrapperExtensions
    {
        public static void Subscribe<TEvent, TMessage>(this IEventBusWrapper @this, 
            Action<TMessage> messageHandler) where TEvent : IEvent<TMessage>
        {
            @this.Subscribe<TEvent, TMessage>(new MessageHandlerAdapter<TMessage>(messageHandler));
        }
        
    }
    
    internal class MessageHandlerAdapter<TMessage> : IMessageHandler<TMessage>
    {
        private readonly Action<TMessage> handler;

        public MessageHandlerAdapter(Action<TMessage> handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Handle(TMessage message)
        {
            Console.WriteLine("Handling message: " + message.GetType());
            handler.Invoke(message);
        }
    }
}