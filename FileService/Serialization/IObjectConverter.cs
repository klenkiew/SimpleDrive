namespace FileService.Serialization
{
    public interface IObjectConverter
    {
        string ToString<TKey>(TKey key);
    }
}