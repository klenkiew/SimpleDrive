using System;
using FileService.Serialization;

namespace FileService.Cache
{
    internal class UniversalCache : IUniversalCache
    {
        private readonly ICache cache;
        private readonly ICacheKeyConverter keyConverter;

        public UniversalCache(ICache cache, ICacheKeyConverter keyConverter)
        {
            this.cache = cache;
            this.keyConverter = keyConverter;
        }

        public TValue Get<TKey, TValue>(TKey key)
        {
            return cache.Get<TValue>(keyConverter.ToString(key));
        }

        public void Set<TKey, TValue>(TKey key, TValue value)
        {
            cache.Set(keyConverter.ToString(key), value);
        }

        public TValue ComputeIfAbsent<TKey, TValue>(TKey key, Func<TValue> valueProvider)
        {
            return cache.ComputeIfAbsent(keyConverter.ToString(key), valueProvider);
        }

        public void Remove<TKey>(TKey key)
        {
            cache.Remove(keyConverter.ToString(key));
        }
    }
}