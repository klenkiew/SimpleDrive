using System;
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
        private readonly IEventBusWrapper eventBus;
        private readonly ILogger<FileLockChangedNotificator> logger;
        private readonly IClientGroupsManager clientGroupsManager;
        
        public FileLockChangedNotificator(
            IEventBusWrapper eventBus, 
            IClientGroupsManager clientGroupsManager, 
            ILoggerFactory loggerFactory)
        {
            this.eventBus = eventBus;
            this.clientGroupsManager = clientGroupsManager;
            this.logger = loggerFactory.CreateLogger<FileLockChangedNotificator>();
            
            eventBus.Subscribe<FileLockChangedEvent, FileLockChangedMessage>(message =>
            {
                logger.LogTrace("Event: " + message.FileId);
                clientGroupsManager.SendToGroup(message.FileId, message);
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
            clientGroupsManager.AddClient(message.FileId, context);
        }

        public void OnClose(IWebSocketContext context, WebSocketCloseStatus closeStatus)
        {
            logger.LogDebug("Connection closed.");
            clientGroupsManager.RemoveClient(context);
        }
    }
}