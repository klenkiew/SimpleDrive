using System;
using StackExchange.Redis;

namespace Redis
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<IConnectionMultiplexer> connectionProvider;
        
        public RedisConnectionFactory(IRedisConfiguration configuration)
        {
            connectionProvider = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configuration.Host));
        }

        public IConnectionMultiplexer Connection => connectionProvider.Value;
    }
}