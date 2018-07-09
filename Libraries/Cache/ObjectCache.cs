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
            return serializer.Deserialize<T>(stringCache.Get(key));
        }

        public void Set<T>(string key, T value)
        {
            stringCache.Set(key, serializer.Serialize(value));
        }

        public T ComputeIfAbsent<T>(string key, Func<T> valueProvider)
        {
            return serializer.Deserialize<T>(stringCache.ComputeIfAbsent(key, () => serializer.Serialize(valueProvider())));
        }

        public void Remove(string key)
        {
            stringCache.Remove(key);
        }
    }
}