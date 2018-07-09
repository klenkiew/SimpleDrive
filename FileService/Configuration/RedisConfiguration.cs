using Redis;

namespace FileService.Configuration
{
    public class RedisConfiguration : IRedisConfiguration
    {
        public string Host { get; set; }
        public ConnectionFailedFallback ConnectionFailedFallback { get; set; }

        // for the configuration binding purposes
        public RedisConfiguration()
        {
        }

        public RedisConfiguration(string host, ConnectionFailedFallback connectionFailedFallback)
        {
            Host = host;
            ConnectionFailedFallback = connectionFailedFallback;
        }
    }

    public enum ConnectionFailedFallback
    {
        Ignore,
        Error,
        InMemoryCache,
        TryStart
    }
}