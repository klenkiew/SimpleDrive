using Redis;

namespace FileService.Configuration
{
    public class RedisConfiguration : IRedisConfiguration
    {
        public string Host { get; set; }

        // for the configuration binding purposes
        public RedisConfiguration()
        {
        }

        public RedisConfiguration(string host)
        {
            Host = host;
        }
    }
}