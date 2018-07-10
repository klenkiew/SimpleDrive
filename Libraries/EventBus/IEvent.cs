namespace EventBus
{
    public interface IEvent<TMessage>
    {
        string Name { get; }
        TMessage Message { get; }
    }

    internal static class EventExtensions
    {
        public static string GetName<T>(this IEvent<T> @this)
        {
            return Events.GetName<IEvent<T>, T>();
        }
    }

    public static class Events
    {
        public static string GetName<TEvent, TMessage>() where TEvent : IEvent<TMessage>
        {
            return typeof(IEvent<TMessage>).Name;
        }
    }
}
