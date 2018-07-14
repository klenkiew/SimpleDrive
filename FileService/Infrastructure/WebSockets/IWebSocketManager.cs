using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    public interface IWebSocketManager
    {
        Task OnWebSocketConnectionReceived(HttpContext context);
    }
}