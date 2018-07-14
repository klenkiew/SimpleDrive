using System.Net.WebSockets;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketObjectMessageSender
    {
        void Send<T>(WebSocket socket, T message);
    }
}