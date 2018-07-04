namespace FileService.Serialization
{
    public interface ICacheKeyConverter
    {
        string ToString<TKey>(TKey key);
    }
}