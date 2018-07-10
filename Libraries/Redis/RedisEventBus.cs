using EventBus;

namespace Redis
{
    public class RedisEventBus : ITypedEventBus<string>
    {
        private readonly IRedisConnectionFactory connectionFactory;
        
        public RedisEventBus(IRedisConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public void Subscribe(string topic, IMessageHandler<string> messageHandler)
        {
            var subscriber = connectionFactory.Connection.GetSubscriber();
            subscriber.Subscribe(topic, (channel, message) => messageHandler.Handle(message));
        }

        public void Publish(string topic, string message)
        {
            connectionFactory.Connection.GetDatabase().Publish(topic, message);
        }
    }
}