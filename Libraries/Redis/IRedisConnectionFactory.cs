using StackExchange.Redis;

namespace Redis
{
    public interface IRedisConnectionFactory
    {
        IConnectionMultiplexer Connection { get; }
    }
}