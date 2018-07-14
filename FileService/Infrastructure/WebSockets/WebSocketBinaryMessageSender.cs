using System;
using System.Net.WebSockets;
using System.Threading;

namespace FileService.Infrastructure.WebSockets
{
    public class WebSocketBinaryMessageSender : IWebSocketBinaryMessageSender
    {
        public void Send(WebSocket socket, byte[] message, WebSocketMessageType messageType)
        {
            socket.SendAsync(new ArraySegment<byte>(message), messageType, true, CancellationToken.None);
        }
    }
}