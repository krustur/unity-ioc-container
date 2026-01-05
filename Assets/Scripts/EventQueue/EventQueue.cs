using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIoC.EventQueue
{
    /// <summary>
    /// High-performance event queue implementation for Unity.
    /// Events are queued and dispatched sequentially to maintain deterministic behavior.
    /// </summary>
    public class EventQueue : IEventQueue
    {
        private readonly Queue<IEvent> _eventQueue;
        private readonly Dictionary<Type, Delegate> _eventHandlers;
        
        public int Count => _eventQueue.Count;
        
        public EventQueue()
        {
            _eventQueue = new Queue<IEvent>(64); // Pre-allocate for performance
            _eventHandlers = new Dictionary<Type, Delegate>(32);
        }
        
        /// <summary>
        /// Queues an event to be dispatched later.
        /// </summary>
        public void QueueEvent(IEvent evt)
        {
            if (evt == null)
            {
                Debug.LogWarning("Attempted to queue a null event.");
                return;
            }
            
            _eventQueue.Enqueue(evt);
        }
        
        /// <summary>
        /// Dispatches all queued events in order (FIFO).
        /// </summary>
        public void DispatchEvents()
        {
            while (_eventQueue.Count > 0)
            {
                var evt = _eventQueue.Dequeue();
                DispatchEvent(evt);
            }
        }
        
        /// <summary>
        /// Subscribes a handler to a specific event type.
        /// </summary>
        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
            {
                Debug.LogWarning($"Attempted to subscribe a null handler for event type {typeof(T).Name}.");
                return;
            }
            
            var eventType = typeof(T);
            
            if (_eventHandlers.TryGetValue(eventType, out var existingDelegate))
            {
                _eventHandlers[eventType] = Delegate.Combine(existingDelegate, handler);
            }
            else
            {
                _eventHandlers[eventType] = handler;
            }
        }
        
        /// <summary>
        /// Unsubscribes a handler from a specific event type.
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            if (handler == null)
            {
                Debug.LogWarning($"Attempted to unsubscribe a null handler for event type {typeof(T).Name}.");
                return;
            }
            
            var eventType = typeof(T);
            
            if (_eventHandlers.TryGetValue(eventType, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, handler);
                
                if (newDelegate == null)
                {
                    _eventHandlers.Remove(eventType);
                }
                else
                {
                    _eventHandlers[eventType] = newDelegate;
                }
            }
        }
        
        /// <summary>
        /// Clears all queued events without dispatching them.
        /// </summary>
        public void Clear()
        {
            _eventQueue.Clear();
        }
        
        /// <summary>
        /// Dispatches a single event to all registered handlers.
        /// </summary>
        private void DispatchEvent(IEvent evt)
        {
            var eventType = evt.GetType();
            
            if (_eventHandlers.TryGetValue(eventType, out var handler))
            {
                try
                {
                    handler.DynamicInvoke(evt);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error dispatching event {evt.EventName}: {ex.Message}");
                }
            }
        }
    }
}
