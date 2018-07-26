namespace EventBus
{
    public interface IEvent<TMessage>
    {
        TMessage Message { get; }
    }

    internal static class EventExtensions
    {
        public static string GetName<TMessage>(this IEvent<TMessage> @this)
        {
            return @this.GetType().Name;
        }
    }

    public static class Events
    {
        public static string GetName<TEvent, TMessage>() where TEvent : IEvent<TMessage>
        {
            return typeof(TEvent).Name;
        }
    }
}
