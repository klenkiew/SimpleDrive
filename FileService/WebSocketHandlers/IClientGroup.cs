using FileService.Infrastructure.WebSockets;

namespace FileService.WebSocketHandlers
{
    internal interface IClientGroup
    {
        void AddClient(IWebSocketContext clientContext);
        void RemoveClient(IWebSocketContext clientContext);
        void SendToAll<T>(T message);
    }
}