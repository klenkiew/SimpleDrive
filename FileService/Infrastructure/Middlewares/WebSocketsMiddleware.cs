using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using FileService.Infrastructure.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FileService.Infrastructure.Middlewares
{
    public class WebSocketsMiddleware : IMiddleware
    {
        private readonly IWebSocketManager webSocketManager;
        private readonly ILogger<WebSocketsMiddleware> logger;

        public WebSocketsMiddleware(
            IWebSocketManager webSocketManager, 
            ILoggerFactory loggerFactory)
        {
            this.webSocketManager = webSocketManager;
            this.logger = loggerFactory.CreateLogger<WebSocketsMiddleware>();
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                await next(context);
            else
                await webSocketManager.OnWebSocketConnectionReceived(context);
        }
    }
}