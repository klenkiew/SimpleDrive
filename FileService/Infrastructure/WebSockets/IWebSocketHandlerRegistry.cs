using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketHandlerRegistry
    {
        IWebSocketHandler<byte[]> GetHandler(PathString path);
    }

    public interface IWebSocketHandlerRegistrar
    {
        void RegisterHandler<T>(PathString path, IWebSocketHandler<T> handler);
    }
}