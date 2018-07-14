using System.Net.WebSockets;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketHandler<in TMessage>
    {
        void OnOpen(IWebSocketContext context);
        void OnMessage(IWebSocketContext context, TMessage message, MessageType type);
        void OnClose(IWebSocketContext context, WebSocketCloseStatus closeStatus);
    }
}