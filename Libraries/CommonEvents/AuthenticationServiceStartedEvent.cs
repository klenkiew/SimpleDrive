using EventBus;

namespace CommonEvents
{
    public class AuthenticationServiceStartedEvent : EventBase<EmptyMessage>
    {
        private AuthenticationServiceStartedEvent(EmptyMessage message) : base(message)
        { }
    }
}