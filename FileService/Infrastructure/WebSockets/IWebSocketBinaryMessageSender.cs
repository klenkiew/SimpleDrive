using System.Net.WebSockets;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketBinaryMessageSender
    {
        void Send(WebSocket socket, byte[] message, WebSocketMessageType messageType);
    }
}