﻿using System;

namespace EventBus
{
    public class EventBusWrapper : IEventBusWrapper
    {
        private readonly IEventBus eventBus;

        public EventBusWrapper(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Publish<TEvent, TMessage>(TEvent @event) where TEvent : IEvent<TMessage>
        {
            eventBus.Publish(@event.GetName(), @event.Message);
        }

        public void Subscribe<TEvent, TMessage>(IMessageHandler<TMessage> messageHandler) where TEvent : IEvent<TMessage>
        {
            eventBus.Subscribe(Events.GetName<IEvent<TMessage>, TMessage>(), messageHandler);
        }
    }
}