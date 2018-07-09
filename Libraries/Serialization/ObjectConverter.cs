namespace Serialization
{
    public class ObjectConverter : IObjectConverter
    {
        private readonly ISerializer serializer;

        public ObjectConverter(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public string ToString<TKey>(TKey key)
        {
            return $"[{key.GetType().Name}]{serializer.Serialize(key)}";
        }
    }
}