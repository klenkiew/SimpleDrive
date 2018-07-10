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

        public void Subscribe<T>(string topic, IMessageHandler<T> messageHandler)
        {
            adaptee.Subscribe(topic, new StringMessageHandler<T>(messageHandler, serializer));
        }
    }

    internal class StringMessageHandler<TOut> : IMessageHandler<string>
    {
        private readonly IMessageHandler<TOut> adaptee;
        private readonly ISerializer serializer;
        
        public StringMessageHandler(IMessageHandler<TOut> adaptee, ISerializer serializer)
        {
            this.adaptee = adaptee;
            this.serializer = serializer;
        }

        public void Handle(string message)
        {
            adaptee.Handle(serializer.Deserialize<TOut>(message));
        }
    }
}