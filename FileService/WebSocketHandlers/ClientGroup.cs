using System.Collections.Concurrent;
using FileService.Infrastructure.WebSockets;

namespace FileService.WebSocketHandlers
{
    internal class ClientGroup
    {
        private string Id { get; }

        // no concurrent set in the standard library so a dictionary with a pointless value is used
        private readonly ConcurrentDictionary<IWebSocketContext, byte> Clients =
            new ConcurrentDictionary<IWebSocketContext, byte>();

        private readonly IWebSocketObjectMessageSender sender;

        public ClientGroup(string id, IWebSocketObjectMessageSender sender)
        {
            Id = id;
            this.sender = sender;
        }

        public void AddClient(IWebSocketContext clientContext)
        {
            Clients.AddOrUpdate(clientContext, 0, (context, @byte) => 0);
        }

        public void RemoveClient(IWebSocketContext clientContext)
        {
            Clients.TryRemove(clientContext, out _);
        }

        public void SendToAll<T>(T message)
        {
            foreach (var client in Clients)
                sender.Send(client.Key.WebSocket, message);
        }
    }
}