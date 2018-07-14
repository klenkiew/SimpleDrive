using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FileService.Infrastructure.WebSockets
{
    public class WebSocketHandlerRegistry : IWebSocketHandlerRegistry
    {
        private readonly IDictionary<PathString, IWebSocketHandler<byte[]>> registry 
            = new Dictionary<PathString, IWebSocketHandler<byte[]>>();

        private bool registrationClosed = false;
        
        public void RegisterHandler(PathString path, IWebSocketHandler<byte[]> handler)
        {
            if (registrationClosed == true)
                throw new InvalidOperationException("New handlers cannot be registered after the first invocation of the GetHandler method");
            
            registry.Add(path, handler);
        }

        public IWebSocketHandler<byte[]> GetHandler(PathString path)
        {
            registrationClosed = true;

            registry.TryGetValue(path, out var handler);
            return handler;
        }
    }
}