using EventBus;

namespace CommonEvents
{
    public class AuthenticationServiceStartedEvent : EventBase<AuthenticationServiceStarted>
    {
        private static readonly AuthenticationServiceStartedEvent instance =
            new AuthenticationServiceStartedEvent(new AuthenticationServiceStarted());
        
        private AuthenticationServiceStartedEvent(AuthenticationServiceStarted message) : base(message)
        { }

        public static AuthenticationServiceStartedEvent Create()
        {
            return instance;
        }
    }
}