using System.Net.WebSockets;
using Serialization;

namespace FileService.Infrastructure.WebSockets
{
    public class WebSocketObjectMessageSender : IWebSocketObjectMessageSender
    {
        private readonly IWebSocketTextMessageSender textSender;
        private readonly ISerializer serializer;

        public WebSocketObjectMessageSender(IWebSocketTextMessageSender textSender, ISerializer serializer)
        {
            this.textSender = textSender;
            this.serializer = serializer;
        }

        public void Send<T>(WebSocket socket, T message)
        {
            textSender.Send(socket, serializer.Serialize(message));
        }
    }
}