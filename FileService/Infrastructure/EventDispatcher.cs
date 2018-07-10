using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace FileService.Infrastructure
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Container container;
        private readonly ILogger<EventDispatcher> logger;

        public EventDispatcher(Container container, ILoggerFactory loggerFactory)
        {
            this.container = container;
            this.logger = loggerFactory.CreateLogger<EventDispatcher>();
        }

        public void SubscribeToEvents()
        {
            List<Type> messageHandlers = container.GetCurrentRegistrations()
                .Where(producer => producer.ServiceType.IsGenericType && 
                                   producer.ServiceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                .Select(producer => producer.ServiceType)
                .ToList();

            logger.LogDebug("Found message handlers:\n" + string.Join('\n', messageHandlers.Select(mh => mh.FullName)));
            
            var eventBus = container.GetService<IEventBusWrapper>();
            foreach (var @interface in messageHandlers)
            {
                Type messageType = @interface.GetGenericArguments()[0];
                Type eventType = typeof(IEvent<>).MakeGenericType(messageType);

                var dispatchHelper = new DispatcherHelper(container, @interface);

                var dispatchMethod = dispatchHelper.GetType().GetMethod("DispatchMessage")
                    .MakeGenericMethod(messageType);
                
                Action<dynamic> handler = message =>
                {
                    try
                    {
                        // the messsage is of dynamic type - cannot invoke the extension method directly
                        LoggerExtensions.LogTrace(logger, "Handler for message: " + message.GetType());
                        dispatchMethod.Invoke(dispatchHelper, new object[] {message});
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Message handler failed to process a message");
                    }
                };
                
                typeof(EventBusWrapperExtensions).GetMethod("Subscribe").MakeGenericMethod(eventType, messageType)
                    .Invoke(null, new object[] {eventBus, handler});                
            }
        }
        
        private class DispatcherHelper
        {
            private readonly Container container;
            private readonly Type handlerInterface;

            public DispatcherHelper(Container container, Type handlerInterface)
            {
                this.container = container;
                this.handlerInterface = handlerInterface;
            }

            public void DispatchMessage<T>(T message)
            {
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    dynamic service = container.GetInstance(@handlerInterface);
                    service.Handle(message);
                }
            }
        }
    }
}