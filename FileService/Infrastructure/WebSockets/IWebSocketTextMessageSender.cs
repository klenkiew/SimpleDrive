using System.Net.WebSockets;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketTextMessageSender
    {
        void Send(WebSocket socket, string message);
    }
}