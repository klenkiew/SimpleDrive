namespace Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T @object);
        T Deserialize<T>(string serialized);
    }
}