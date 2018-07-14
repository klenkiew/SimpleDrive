using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketHandlerRegistry
    {
        void RegisterHandler(PathString path, IWebSocketHandler<byte[]> handler);
        IWebSocketHandler<byte[]> GetHandler(PathString path);
    }
}