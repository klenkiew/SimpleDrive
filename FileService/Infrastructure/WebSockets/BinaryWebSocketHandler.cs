using System.IO;
using System.Net.WebSockets;

namespace FileService.Infrastructure.WebSockets
{
    public class BinaryWebSocketHandler : IWebSocketHandler<byte[]>
    {
        private readonly IWebSocketHandler<string> handler;

        public BinaryWebSocketHandler(IWebSocketHandler<string> handler)
        {
            this.handler = handler;
        }

        public void OnOpen(IWebSocketContext context)
        {
            handler.OnOpen(context);
        }

        public void OnMessage(IWebSocketContext context, byte[] binaryMessage, MessageType type)
        {
            string message;
            using (var stream = new MemoryStream(binaryMessage))
            using (var reader = new StreamReader(stream))
                message = reader.ReadToEnd();
            
            handler.OnMessage(context, message, type);
        }

        public void OnClose(IWebSocketContext context, WebSocketCloseStatus closeStatus)
        {
            handler.OnClose(context, closeStatus);
        }
    }
}