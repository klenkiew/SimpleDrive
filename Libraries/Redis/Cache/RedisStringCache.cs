using System;
using Cache;
using StackExchange.Redis;

namespace Redis.Cache
{
    public class RedisStringCache : ICache<string>
    {
        private readonly IRedisConnectionFactory redisConnectionFactory;

        private IDatabase Redis => redisConnectionFactory.Connection.GetDatabase();

        public RedisStringCache(IRedisConnectionFactory redisConnectionFactory)
        {
            this.redisConnectionFactory = redisConnectionFactory;
        }

        public string Get(string key)
        {
            return Redis.StringGet(key);
        }

        public void Set(string key, string value, TimeSpan? expiry)
        {
            Redis.StringSet(key, value, expiry);
        }

        public string ComputeIfAbsent(string key, Func<string> valueProvider, TimeSpan? expiry)
        {
            var value = Get(key);
            if (value != null)
                return value;

            value = valueProvider();
            Set(key, value, expiry);
            return value;
        }

        public void Remove(string key)
        {
            Redis.KeyDelete(key);
        }
    }
}