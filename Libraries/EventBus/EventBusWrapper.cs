namespace EventBus
{
    public class EventBusWrapper : IEventBusWrapper
    {
        private readonly IEventBus eventBus;

        public EventBusWrapper(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Publish<TEvent, TMessage>(TMessage message) where TEvent : IEvent<TMessage>
        {
            eventBus.Publish(Events.GetName<TEvent, TMessage>(), message);
        }

        public void Subscribe<TEvent, TMessage>(IEventHandler<TEvent, TMessage> eventHandler) where TEvent : IEvent<TMessage>
        {
            eventBus.Subscribe(Events.GetName<TEvent, TMessage>(), eventHandler);
        }
    }
}