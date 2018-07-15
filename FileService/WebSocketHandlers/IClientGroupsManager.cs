using FileService.Infrastructure.WebSockets;

namespace FileService.WebSocketHandlers
{
    public interface IClientGroupsManager
    {
        void AddClient(string groupId, IWebSocketContext client);
        void RemoveClient(IWebSocketContext client);
        void SendToGroup<T>(string groupId, T message);
    }
}