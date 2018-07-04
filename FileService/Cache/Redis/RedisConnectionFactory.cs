using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace FileService.Cache.Redis
{
    internal class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<IConnectionMultiplexer> connectionProvider;
        
        public RedisConnectionFactory(IConfiguration configuration)
        {
            connectionProvider = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configuration["RedisHost"]));
        }

        public IConnectionMultiplexer Connection => connectionProvider.Value;
    }
}