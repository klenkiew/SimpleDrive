using Serialization;

namespace EventBus
{
    public class StringEventBusAdapter : IEventBus
    {
        private readonly ITypedEventBus<string> adaptee;
        private readonly ISerializer serializer;

        public StringEventBusAdapter(ITypedEventBus<string> adaptee, ISerializer serializer)
        {
            this.adaptee = adaptee;
            this.serializer = serializer;
        }

        public void Publish<T>(string topic, T message)
        {
            adaptee.Publish(topic, serializer.Serialize(message));
        }

        public void Subscribe<TEvent, TMessage>(string topic, IEventHandler<TEvent, TMessage> eventHandler) where TEvent : IEvent<TMessage>
        {
            adaptee.Subscribe(topic, new StringEventHandler<TEvent, TMessage>(eventHandler, serializer));
        }
    }

    internal class StringEventHandler<TEvent, TMessage> : IEventHandler<StringEvent, string> where TEvent : IEvent<TMessage>
    {
        private readonly IEventHandler<TEvent, TMessage> adaptee;
        private readonly ISerializer serializer;
        
        public StringEventHandler(IEventHandler<TEvent, TMessage> adaptee, ISerializer serializer)
        {
            this.adaptee = adaptee;
            this.serializer = serializer;
        }

        public void Handle(string message)
        {
            adaptee.Handle(serializer.Deserialize<TMessage>(message));
        }
    }

    internal class StringEvent : IEvent<string>
    {
        public string Message { get; }
    }
}