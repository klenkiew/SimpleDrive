using System;
using Microsoft.Extensions.Caching.Memory;

namespace Cache
{
    public class InMemoryObjectCache : ICache
    {
        private readonly IMemoryCache cache;

        public InMemoryObjectCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public TValue Get<TValue>(string key)
        {
            return cache.Get<TValue>(key);
        }

        public void Set<TValue>(string key, TValue value, TimeSpan? expiry)
        {
            if (expiry == null)
                cache.Set(key, value);
            else
                cache.Set(key, value, expiry.Value);
        }

        public TValue ComputeIfAbsent<TValue>(string key, Func<TValue> valueProvider, TimeSpan? expiry = null)
        {
            if (expiry == null) 
                return cache.GetOrCreate(key, (cacheEntry) => valueProvider());
            
            // IMemoryCache does allow passing TTL for a cache entry for the Set method
            // but not for ComputeIfAbsent... - ¯\_(ツ)_/¯
            var value = Get<TValue>(key);
            if (value != null)
                return value;

            value = valueProvider();
            Set(key, value, expiry.Value);
            return value;
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}