using System;
using EventBus;
using Microsoft.Extensions.Logging;

namespace Redis
{
    public class RedisEventBus : ITypedEventBus<string>
    {
        private readonly IRedisConnectionFactory connectionFactory;
        private readonly ILogger<RedisEventBus> logger;

        public RedisEventBus(IRedisConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            this.connectionFactory = connectionFactory;
            this.logger = loggerFactory.CreateLogger<RedisEventBus>();
        }

        public void Subscribe(string topic, IMessageHandler<string> messageHandler)
        {
            logger.LogDebug("[Redis] Subscribing: " + topic);
            var subscriber = connectionFactory.Connection.GetSubscriber();
            subscriber.Subscribe(topic, (channel, message) =>
            {
                try
                {
                    logger.LogTrace("[Redis] Handler for: " + channel);
                    messageHandler.Handle(message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"[Redis] Unhandled exception in message handler for topic {topic}");
                }
            });
        }

        public void Publish(string topic, string message)
        {
            logger.LogDebug("[Redis] Publishing: " + topic);
            connectionFactory.Connection.GetDatabase().Publish(topic, message);
        }
    }
}