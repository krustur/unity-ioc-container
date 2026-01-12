# EventQueue Quick Start

This document provides a quick reference for using the EventQueue system in your Unity project.

## Installation

1. The EventQueue system is already included in the project under `Assets/Scripts/EventQueue/`
2. No additional setup is required to use it standalone
3. For IoC integration, register it in your `GameBootstrap.cs`

## Quick Examples

### Standalone Usage

```csharp
using UnityEngine;
using UnityIoC.EventQueue;

public class QuickExample : MonoBehaviour
{
    private IEventQueue _eventQueue;
    
    void Start()
    {
        // Create event queue
        _eventQueue = new EventQueue();
        
        // Subscribe to events
        _eventQueue.Subscribe<MyEvent>(OnMyEvent);
        
        // Queue an event
        _eventQueue.QueueEvent(new MyEvent { Data = "Hello" });
        
        // Dispatch events
        _eventQueue.DispatchEvents();
    }
    
    void OnMyEvent(MyEvent evt)
    {
        Debug.Log($"Event received: {evt.Data}");
    }
}

public class MyEvent : IEvent
{
    public string EventName => "MyEvent";
    public string Data { get; set; }
}
```

### IoC Container Integration

```csharp
// In GameBootstrap.cs InitializeContainer():
_container.Register<IEventQueue, EventQueue>(ServiceLifetime.Singleton);

// In your service:
public class MyService
{
    public MyService(IEventQueue eventQueue)
    {
        eventQueue.Subscribe<MyEvent>(OnMyEvent);
    }
    
    void OnMyEvent(MyEvent evt) { /* handle */ }
}
```

## Key Methods

- `QueueEvent(IEvent)` - Add event to queue
- `DispatchEvents()` - Process all queued events
- `Subscribe<T>(Action<T>)` - Listen to event type
- `Unsubscribe<T>(Action<T>)` - Stop listening
- `Clear()` - Remove all queued events
- `Count` - Get number of queued events

## Common Event Types

Create events by implementing `IEvent`:

```csharp
public class PlayerDiedEvent : IEvent
{
    public string EventName => "PlayerDied";
    public Vector3 Position { get; set; }
}

public class LevelCompletedEvent : IEvent
{
    public string EventName => "LevelCompleted";
    public int Score { get; set; }
    public float Time { get; set; }
}

public class ItemPickedUpEvent : IEvent
{
    public string EventName => "ItemPickedUp";
    public string ItemId { get; set; }
    public int Quantity { get; set; }
}
```

## Best Practices

1. **Unsubscribe in OnDestroy**: Prevent memory leaks
2. **Dispatch Once Per Frame**: Call `DispatchEvents()` in `LateUpdate()`
3. **Keep Events Small**: Minimize data in events
4. **Use Descriptive Names**: Make event purpose clear
5. **Centralize Event Types**: Keep all events in one namespace

## See Full Documentation

For complete documentation, see [EVENTQUEUE_GUIDE.md](EVENTQUEUE_GUIDE.md)
