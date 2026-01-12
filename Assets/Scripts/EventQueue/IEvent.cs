namespace UnityIoC.EventQueue
{
    /// <summary>
    /// Base interface for all events in the event queue system.
    /// Implement this interface to create custom event types.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The name or type identifier of the event.
        /// </summary>
        string EventName { get; }
    }
}
