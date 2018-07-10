namespace EventBus
{
    public interface ISubscriber
    {
        void Subscribe<T>(string topic, IMessageHandler<T> messageHandler);
    }
}