using Newtonsoft.Json;

namespace Serialization
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}