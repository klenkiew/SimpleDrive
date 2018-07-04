namespace FileService.Serialization
{
    internal class CacheKeyConverter : ICacheKeyConverter
    {
        private readonly ISerializer serializer;

        public CacheKeyConverter(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public string ToString<TKey>(TKey key)
        {
            return $"[{typeof(TKey).Name}]{serializer.Serialize(key)}";
        }
    }
}