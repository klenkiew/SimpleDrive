using System;

namespace Cache
{
    public interface ICache
    {
        TValue Get<TValue>(string key);
        void Set<TValue>(string key, TValue value);
        TValue ComputeIfAbsent<TValue>(string key, Func<TValue> valueProvider);
        void Remove(string key);
    }

    public interface ICache<TValue>
    {
        TValue Get(string key);
        void Set(string key, TValue value);
        TValue ComputeIfAbsent(string key, Func<TValue> valueProvider);
        void Remove(string key);
    }
    
    public interface IUniversalCache
    {
        TValue Get<TKey, TValue>(TKey key);
        void Set<TKey, TValue>(TKey key, TValue value);
        TValue ComputeIfAbsent<TKey, TValue>(TKey key, Func<TValue> valueProvider);
        void Remove<TKey>(TKey key);
    }
}