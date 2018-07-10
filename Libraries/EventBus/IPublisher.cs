namespace EventBus
{
    public interface IPublisher
    {
        void Publish<T>(string topic, T message);
    }
}