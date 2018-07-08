using System;
using FileService.Configuration;
using StackExchange.Redis;

namespace FileService.Cache.Redis
{
    internal class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<IConnectionMultiplexer> connectionProvider;
        
        public RedisConnectionFactory(IRedisConfiguration configuration)
        {
            connectionProvider = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configuration.Host));
        }

        public IConnectionMultiplexer Connection => connectionProvider.Value;
    }
}