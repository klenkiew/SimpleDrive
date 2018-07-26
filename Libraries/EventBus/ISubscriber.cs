namespace EventBus
{
    public interface ISubscriber
    {
        void Subscribe<TEvent, TMessage>(string topic, IEventHandler<TEvent, TMessage> eventHandler)
            where TEvent : IEvent<TMessage>;
    }
}