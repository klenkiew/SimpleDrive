namespace Serialization
{
    public interface IObjectConverter
    {
        string ToString<TKey>(TKey key);
    }
}