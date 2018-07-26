namespace EventBus
{
    public interface ITypedEventBus<TMessage>
    {
        void Subscribe<TEvent>(string topic, IEventHandler<TEvent, TMessage> eventHandler) where TEvent : IEvent<TMessage>;
        void Publish(string topic, TMessage message);
        
    }
}