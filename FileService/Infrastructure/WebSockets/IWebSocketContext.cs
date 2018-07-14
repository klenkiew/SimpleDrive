using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketContext
    {
        HttpContext HttpContext { get; }
        WebSocket WebSocket { get; }
    }
}