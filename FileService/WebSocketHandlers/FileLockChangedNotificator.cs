using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using EventBus;
using FileService.Dto;
using FileService.Events;
using FileService.Infrastructure.WebSockets;
using Microsoft.Extensions.Logging;

namespace FileService.WebSocketHandlers
{
    public class FileLockChangedNotificator : IWebSocketHandler<CurrentLockNotificationsSubscriptionMessage>
    {
        private readonly IWebSocketObjectMessageSender sender;
        private readonly IEventBusWrapper eventBus;
        private readonly ILogger<FileLockChangedNotificator> logger;
        
        private readonly ConcurrentDictionary<string, ClientGroup> clientGroupsById =
            new ConcurrentDictionary<string, ClientGroup>();

        private readonly ConcurrentDictionary<IWebSocketContext, ClientGroup> clientsToGroupsMap =
            new ConcurrentDictionary<IWebSocketContext, ClientGroup>();

        public FileLockChangedNotificator(
            IWebSocketObjectMessageSender sender, 
            IEventBusWrapper eventBus, 
            ILoggerFactory loggerFactory)
        {
            this.sender = sender;
            this.eventBus = eventBus;
            this.logger = loggerFactory.CreateLogger<FileLockChangedNotificator>();
            
            eventBus.Subscribe<IEvent<FileLockChangedMessage>, FileLockChangedMessage>(message =>
            {
                clientGroupsById.TryGetValue(message.FileId, out var group);
                group?.SendToAll(message);
            });
        }

        public void OnOpen(IWebSocketContext context)
        {
            logger.LogDebug("Connection opened.");
        }

        public void OnMessage(IWebSocketContext context, CurrentLockNotificationsSubscriptionMessage message,
            MessageType type)
        {
            logger.LogDebug("Received message: subscription for " + message.FileId);
            var group = clientGroupsById.GetOrAdd(message.FileId, id => new ClientGroup(message.FileId, sender));
            clientsToGroupsMap[context] = group;
            group.AddClient(context);
        }

        public void OnClose(IWebSocketContext context, WebSocketCloseStatus closeStatus)
        {
            logger.LogDebug("Connection closed.");
            clientsToGroupsMap.TryRemove(context, out var group);
            group?.RemoveClient(context);
        }
    }
}