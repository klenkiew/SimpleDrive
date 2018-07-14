using System.Net.WebSockets;
using System.Text;

namespace FileService.Infrastructure.WebSockets
{
    public class WebSocketTextMessageSender : IWebSocketTextMessageSender
    {
        private readonly IWebSocketBinaryMessageSender binarySender;

        public WebSocketTextMessageSender(IWebSocketBinaryMessageSender binarySender)
        {
            this.binarySender = binarySender;
        }

        public void Send(WebSocket socket, string message)
        {
            binarySender.Send(socket, Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text);
        }
    }
}