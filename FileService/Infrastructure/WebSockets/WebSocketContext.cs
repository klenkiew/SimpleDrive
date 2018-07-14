using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    class WebSocketContext : IWebSocketContext
    {
        public HttpContext HttpContext { get; }
        public WebSocket WebSocket { get; }

        public WebSocketContext(HttpContext httpContext, WebSocket webSocket)
        {
            HttpContext = httpContext;
            WebSocket = webSocket;
        }
    }
}