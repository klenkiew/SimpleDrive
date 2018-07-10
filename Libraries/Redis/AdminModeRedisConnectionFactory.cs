using System;
using StackExchange.Redis;

namespace Redis
{
    public class AdminModeRedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<IConnectionMultiplexer> connectionProvider;
        
        public AdminModeRedisConnectionFactory(IRedisConfiguration configuration)
        {
            connectionProvider = new Lazy<IConnectionMultiplexer>(() =>
            {
                var options = new ConfigurationOptions()
                {
                    EndPoints = {configuration.Host},
                    AllowAdmin = true
                };
                return ConnectionMultiplexer.Connect(options);
            });
        }

        public IConnectionMultiplexer Connection => connectionProvider.Value;
    }
}