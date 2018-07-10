namespace FileService.Configuration
{
    public class CacheConfiguration
    {
        public CacheType CacheType { get; set; }
    }

    public enum CacheType
    {
        Redis,
        InMemory,
        None
    }
}