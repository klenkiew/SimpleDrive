using Newtonsoft.Json;

namespace FileService.Serialization
{
    internal class JsonSerializer: ISerializer
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