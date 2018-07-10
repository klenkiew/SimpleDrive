using System;
using Redis;

namespace AuthenticationService.Configuration
{
    public class RedisConfiguration : IRedisConfiguration
    {
        public string Host { get; }

        public RedisConfiguration(string host)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
        }
    }
}