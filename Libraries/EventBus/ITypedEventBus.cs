namespace EventBus
{
    public interface ITypedEventBus<TMessage>
    {
        void Subscribe(string topic, IMessageHandler<TMessage> messageHandler);
        void Publish(string topic, TMessage message);
        
    }
}