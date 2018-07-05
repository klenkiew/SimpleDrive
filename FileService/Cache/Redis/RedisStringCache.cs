﻿using System;
using StackExchange.Redis;

namespace FileService.Cache.Redis
{
    internal class RedisStringCache : ICache<string>
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

        public void Set(string key, string value)
        {
            Redis.StringSet(key, value);
        }

        public string ComputeIfAbsent(string key, Func<string> valueProvider)
        {
            var value = Get(key);
            if (value != null)
                return value;

            value = valueProvider();
            Set(key, value);
            return value;
        }

        public void Remove(string key)
        {
            Redis.KeyDelete(key);
        }
    }
}