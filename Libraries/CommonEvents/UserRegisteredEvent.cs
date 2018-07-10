using EventBus;

namespace CommonEvents
{
    public class UserRegisteredEvent : EventBase<UserInfo>
    {
        public UserRegisteredEvent(UserInfo message) : base(message) { }
    }

    public class UserInfo
    {
        public string Id { get; }
        public string Username { get; }
        public string Email { get; }

        public UserInfo(string id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
        }
    }
}
