namespace EventBus
{
    public interface IEventHandler<TEvent, TMessage> where TEvent : IEvent<TMessage>
    {
        void Handle(TMessage message);
    }
}