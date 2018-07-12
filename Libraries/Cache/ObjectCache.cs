using System;
using Serialization;

namespace Cache
{
    public class ObjectCache : ICache
    {
        private readonly ISerializer serializer;
        private readonly ICache<string> stringCache;

        public ObjectCache(ISerializer serializer, ICache<string> stringCache)
        {
            this.serializer = serializer;
            this.stringCache = stringCache;
        }

        public T Get<T>(string key)
        {
            var value = stringCache.Get(key);
            return value != null ? serializer.Deserialize<T>(value) : default(T);
        }

        public void Set<T>(string key, T value, TimeSpan? expiry)
        {
            stringCache.Set(key, serializer.Serialize(value), expiry);
        }

        public T ComputeIfAbsent<T>(string key, Func<T> valueProvider, TimeSpan? expiry)
        {
            var value = stringCache.ComputeIfAbsent(key, () => serializer.Serialize(valueProvider()), expiry);
            return serializer.Deserialize<T>(value);
        }

        public void Remove(string key)
        {
            stringCache.Remove(key);
        }
    }
}