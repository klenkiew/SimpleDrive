using System.Net.WebSockets;
using Serialization;

namespace FileService.Infrastructure.WebSockets
{
    public class SerializingWebSocketHandler<TMessage> : IWebSocketHandler<string>
    {
        private readonly IWebSocketHandler<TMessage> handler;
        private readonly ISerializer serializer;

        public SerializingWebSocketHandler(IWebSocketHandler<TMessage> handler, ISerializer serializer)
        {
            this.handler = handler;
            this.serializer = serializer;
        }

        public void OnOpen(IWebSocketContext context)
        {
            handler.OnOpen(context);
        }

        public void OnMessage(IWebSocketContext context, string message, MessageType type)
        {
            var deserializedMessage = serializer.Deserialize<TMessage>(message);
            handler.OnMessage(context, deserializedMessage, type);
        }

        public void OnClose(IWebSocketContext context, WebSocketCloseStatus closeStatus)
        {
            handler.OnClose(context, closeStatus);
        }
    }
}