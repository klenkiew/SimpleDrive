using System;

namespace EventBus
{
    public abstract class EventBase<TMessage> : IEvent<TMessage>
    {
        public string Name { get; }
        public TMessage Message { get; }

        protected EventBase(TMessage message)
        {
            Name = this.GetName();
            Message = message != null ? message : throw new ArgumentNullException(nameof(message));
        }
    }
}