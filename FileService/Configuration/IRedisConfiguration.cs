namespace FileService.Configuration
{
    public interface IRedisConfiguration
    {
        string Host { get; }
        ConnectionFailedFallback ConnectionFailedFallback { get; }
    }
}