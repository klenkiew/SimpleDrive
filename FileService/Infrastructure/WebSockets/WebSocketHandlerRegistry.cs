using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Serialization;
using SimpleInjector;

namespace FileService.Infrastructure.WebSockets
{
    public class WebSocketHandlerRegistry : IWebSocketHandlerRegistry, IWebSocketHandlerRegistrar
    {
        private readonly IDictionary<PathString, IWebSocketHandler<byte[]>> registry 
            = new Dictionary<PathString, IWebSocketHandler<byte[]>>();

        private bool registrationClosed = false;

        private readonly Container container;
        
        public WebSocketHandlerRegistry(Container container)
        {
            this.container = container;
        }

        public void RegisterHandler<T>(PathString path, IWebSocketHandler<T> handler)
        {
            if (registrationClosed)
                throw new InvalidOperationException("New handlers cannot be registered after the first invocation of the GetHandler method");

            
            if (typeof(T) == typeof(string))
            {
                RegisterStringHandler(path, (IWebSocketHandler<string>) handler);
            }
            else if (typeof(T) == typeof(byte[]))
            {
                RegisterBinaryHandler(path, (IWebSocketHandler<byte[]>) handler);
            }
            else
                RegisterObjectHandler(path, handler);
        }

        public IWebSocketHandler<byte[]> GetHandler(PathString path)
        {
            registrationClosed = true;

            registry.TryGetValue(path, out var handler);
            return handler;
        }

        private void RegisterBinaryHandler(PathString path, IWebSocketHandler<byte[]> handler)
        {
            registry.Add(path, handler);
        }

        private void RegisterStringHandler(PathString path, IWebSocketHandler<string> handler)
        {
            var binaryHandler = new BinaryWebSocketHandler(handler);
            registry.Add(path, binaryHandler);
        }

        private void RegisterObjectHandler<T>(PathString path, IWebSocketHandler<T> handler)
        {
            var binaryHandler = new BinaryWebSocketHandler(new SerializingWebSocketHandler<T>(
                handler, container.GetInstance<ISerializer>()));
            
            registry.Add(path, binaryHandler);
        }
    }
}