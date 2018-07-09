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

        public void Set<TValue>(string key, TValue value)
        {
            cache.Set(key, value);
        }

        public TValue ComputeIfAbsent<TValue>(string key, Func<TValue> valueProvider)
        {
            return cache.GetOrCreate(key, (cacheEntry) => valueProvider());
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}