using System;

namespace UnityIoC.EventQueue
{
    /// <summary>
    /// Interface for the event queue system that allows events to be queued and dispatched sequentially.
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Queues an event to be dispatched later.
        /// </summary>
        /// <param name="evt">The event to queue.</param>
        void QueueEvent(IEvent evt);
        
        /// <summary>
        /// Dispatches all queued events in order (FIFO).
        /// </summary>
        void DispatchEvents();
        
        /// <summary>
        /// Subscribes a handler to a specific event type.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to.</typeparam>
        /// <param name="handler">The handler to invoke when the event is dispatched.</param>
        void Subscribe<T>(Action<T> handler) where T : IEvent;
        
        /// <summary>
        /// Unsubscribes a handler from a specific event type.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from.</typeparam>
        /// <param name="handler">The handler to remove.</param>
        void Unsubscribe<T>(Action<T> handler) where T : IEvent;
        
        /// <summary>
        /// Clears all queued events without dispatching them.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Gets the number of events currently queued.
        /// </summary>
        int Count { get; }
    }
}
