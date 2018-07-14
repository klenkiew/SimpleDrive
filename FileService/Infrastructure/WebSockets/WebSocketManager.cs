using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly IWebSocketHandlerRegistry handlerRegistry;

        public WebSocketManager(IWebSocketHandlerRegistry handlerRegistry)
        {
            this.handlerRegistry = handlerRegistry;
        }

        public async Task OnWebSocketConnectionReceived(HttpContext httpContext)
        {
            EnsureWebSocketConnection(httpContext);
            var handler = FindHandler(httpContext);

            using (var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync())
            {
                var webSocketContext = new WebSocketContext(httpContext, webSocket);
                handler.OnOpen(webSocketContext);

                await HandleConnection(webSocketContext, handler);

                if (webSocket.CloseStatus != null)
                    handler.OnClose(webSocketContext, webSocket.CloseStatus.Value);
            }
        }

        private async Task HandleConnection(IWebSocketContext context, IWebSocketHandler<byte[]> handler)
        {
            var buffer = new byte[1024 * 4];
            var webSocket = context.WebSocket;
            while (webSocket.State == WebSocketState.Open)
            {
                var (message, messageType) = await ReceiveMessage(webSocket, buffer);
                
                if (messageType == WebSocketMessageType.Close)
                    continue;

                var type = messageType == WebSocketMessageType.Binary ? MessageType.Binary : MessageType.Text;
                handler.OnMessage(context, message, type);
            }
        }

        private static async Task<(byte[] message, WebSocketMessageType messageType)> ReceiveMessage(WebSocket webSocket, byte[] buffer)
        {
            byte[] message;
            WebSocketReceiveResult result;
            
            using (var stream = new MemoryStream())
            {
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.CloseStatus != null || result.MessageType == WebSocketMessageType.Close)
                        return (null, WebSocketMessageType.Close);

                    await stream.WriteAsync(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                message = stream.ToArray();
            }

            return (message, result.MessageType);
        }

        private IWebSocketHandler<byte[]> FindHandler(HttpContext context)
        {
            var path = context.Request.Path;
            return handlerRegistry.GetHandler(path) 
                   ?? throw new InvalidOperationException($"No web socket handler found for path: '{path}'");
        }

        private void EnsureWebSocketConnection(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                throw new InvalidOperationException("Web socket manager can handle only web socket connections.");
        }
    }
}