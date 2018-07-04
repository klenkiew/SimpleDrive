using StackExchange.Redis;

namespace FileService.Cache.Redis
{
    public interface IRedisConnectionFactory
    {
        IConnectionMultiplexer Connection { get; }
    }
}