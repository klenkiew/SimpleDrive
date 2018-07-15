using System.Collections.Concurrent;
using FileService.Infrastructure.WebSockets;

namespace FileService.WebSocketHandlers
{
    public class ClientGroupsManager : IClientGroupsManager
    {
        private readonly IWebSocketObjectMessageSender sender;
        
        private readonly ConcurrentDictionary<string, IClientGroup> clientGroupsById =
            new ConcurrentDictionary<string, IClientGroup>();

        private readonly ConcurrentDictionary<IWebSocketContext, IClientGroup> clientsToGroupsMap =
            new ConcurrentDictionary<IWebSocketContext, IClientGroup>();

        public ClientGroupsManager(IWebSocketObjectMessageSender sender)
        {
            this.sender = sender;
        }

        public void AddClient(string groupId, IWebSocketContext client)
        {
            var group = clientGroupsById.GetOrAdd(groupId, id => new ClientGroup(groupId, sender));
            clientsToGroupsMap[client] = group;
            group.AddClient(client);
        }

        public void RemoveClient(IWebSocketContext client)
        {
            clientsToGroupsMap.TryRemove(client, out var group);
            group?.RemoveClient(client);
        }
        
        public void SendToGroup<T>(string groupId, T message)
        {
            clientGroupsById.TryGetValue(groupId, out var group);
            group?.SendToAll(message);
        }
    }
}