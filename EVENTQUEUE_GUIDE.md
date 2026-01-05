# EventQueue System Guide

## Overview

The EventQueue system provides a robust, high-performance event management solution for Unity games. It allows you to queue events and dispatch them sequentially, ensuring deterministic behavior and decoupled communication between game systems.

## Key Features

- **Sequential Event Processing**: Events are dispatched in FIFO (First-In-First-Out) order
- **Type-Safe Event Handling**: Strong typing for events and handlers
- **High Performance**: Pre-allocated collections minimize garbage collection
- **IoC Container Integration**: Works seamlessly with the Unity IoC Container
- **Decoupled Architecture**: Systems communicate without direct dependencies
- **Memory Safe**: Proper subscription cleanup prevents memory leaks

## Core Components

### IEvent Interface

Base interface for all events. Every event must implement this interface.

```csharp
public interface IEvent
{
    string EventName { get; }
}
```

### IEventQueue Interface

Defines the contract for the event queue system.

```csharp
public interface IEventQueue
{
    void QueueEvent(IEvent evt);
    void DispatchEvents();
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;
    void Clear();
    int Count { get; }
}
```

### EventQueue Class

High-performance implementation of the event queue.

## Getting Started

### 1. Define Custom Events

Create event classes that implement `IEvent`:

```csharp
using UnityIoC.EventQueue;
using UnityEngine;

public class PlayerSpawnedEvent : IEvent
{
    public string EventName => "PlayerSpawned";
    public Vector3 Position { get; set; }
    public string PlayerName { get; set; }
    
    public PlayerSpawnedEvent(Vector3 position, string playerName)
    {
        Position = position;
        PlayerName = playerName;
    }
}

public class ItemCollectedEvent : IEvent
{
    public string EventName => "ItemCollected";
    public string ItemName { get; set; }
    public int Points { get; set; }
    
    public ItemCollectedEvent(string itemName, int points)
    {
        ItemName = itemName;
        Points = points;
    }
}
```

### 2. Register EventQueue in IoC Container

In your `GameBootstrap.cs`, register the EventQueue as a singleton:

```csharp
private void InitializeContainer()
{
    _container = new Container();
    _container.RegisterInstance<IContainer>(_container);
    
    // Register EventQueue as a singleton
    _container.Register<IEventQueue, EventQueue>(ServiceLifetime.Singleton);
    
    // Register other services...
}
```

### 3. Inject EventQueue into Services

Use constructor injection to access the EventQueue in your services:

```csharp
public class GameEventService : IGameEventService
{
    private readonly IEventQueue _eventQueue;
    
    public GameEventService(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
    }
    
    public void TriggerItemCollection(string itemName, int points)
    {
        var evt = new ItemCollectedEvent(itemName, points);
        _eventQueue.QueueEvent(evt);
    }
}
```

### 4. Subscribe to Events

Subscribe to specific event types in your service constructors or initialization:

```csharp
public class ScoreService : IScoreService
{
    private readonly IEventQueue _eventQueue;
    private int _score;
    
    public ScoreService(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
        _eventQueue.Subscribe<ItemCollectedEvent>(OnItemCollected);
    }
    
    private void OnItemCollected(ItemCollectedEvent evt)
    {
        _score += evt.Points;
        Debug.Log($"Score: {_score}");
    }
}
```

### 5. Queue and Dispatch Events

Queue events during gameplay and dispatch them at appropriate times:

```csharp
public class GameController : MonoBehaviour
{
    private IEventQueue _eventQueue;
    
    private void Update()
    {
        // Game logic that queues events
        if (PlayerCollectedItem())
        {
            _eventQueue.QueueEvent(new ItemCollectedEvent("Coin", 10));
        }
        
        // Dispatch events at end of frame
        _eventQueue.DispatchEvents();
    }
}
```

## Usage Patterns

### Pattern 1: Basic Event Queue Usage

Simple standalone usage without IoC container:

```csharp
public class SimpleExample : MonoBehaviour
{
    private IEventQueue _eventQueue;
    
    private void Awake()
    {
        _eventQueue = new EventQueue();
        _eventQueue.Subscribe<ItemCollectedEvent>(OnItemCollected);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _eventQueue.QueueEvent(new ItemCollectedEvent("Coin", 10));
            _eventQueue.DispatchEvents();
        }
    }
    
    private void OnItemCollected(ItemCollectedEvent evt)
    {
        Debug.Log($"Collected: {evt.ItemName}");
    }
    
    private void OnDestroy()
    {
        _eventQueue.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
    }
}
```

### Pattern 2: IoC Container Integration

Full integration with dependency injection:

```csharp
// In GameBootstrap.cs
private void InitializeContainer()
{
    _container.Register<IEventQueue, EventQueue>(ServiceLifetime.Singleton);
    _container.Register<IPlayerService, PlayerService>(ServiceLifetime.Singleton);
    _container.Register<IInventoryService, InventoryService>(ServiceLifetime.Singleton);
}

// Service implementation
public class PlayerService : IPlayerService
{
    private readonly IEventQueue _eventQueue;
    
    public PlayerService(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
    }
    
    public void CollectItem(string itemName, int points)
    {
        _eventQueue.QueueEvent(new ItemCollectedEvent(itemName, points));
    }
}

public class InventoryService : IInventoryService
{
    public InventoryService(IEventQueue eventQueue)
    {
        eventQueue.Subscribe<ItemCollectedEvent>(OnItemCollected);
    }
    
    private void OnItemCollected(ItemCollectedEvent evt)
    {
        // Add item to inventory
    }
}
```

### Pattern 3: Centralized Event Processing

Process events at specific points in the game loop:

```csharp
public class GameLoop : MonoBehaviour
{
    private IEventQueue _eventQueue;
    
    private void Update()
    {
        // 1. Handle input
        ProcessInput();
        
        // 2. Update game logic
        UpdateGameLogic();
        
        // 3. Dispatch all queued events
        _eventQueue.DispatchEvents();
        
        // 4. Update visual state based on event results
        UpdateVisuals();
    }
}
```

### Pattern 4: Event Cascading

Events can trigger other events:

```csharp
public class ComboSystem
{
    private IEventQueue _eventQueue;
    private int _comboCount;
    
    public ComboSystem(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
        _eventQueue.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
    }
    
    private void OnEnemyDefeated(EnemyDefeatedEvent evt)
    {
        _comboCount++;
        
        if (_comboCount >= 3)
        {
            // Trigger a combo event
            _eventQueue.QueueEvent(new ComboAchievedEvent(_comboCount));
            _comboCount = 0;
        }
    }
}
```

## Best Practices

### 1. Subscribe and Unsubscribe Properly

Always unsubscribe to prevent memory leaks:

```csharp
private void OnEnable()
{
    _eventQueue.Subscribe<MyEvent>(OnMyEvent);
}

private void OnDisable()
{
    _eventQueue.Unsubscribe<MyEvent>(OnMyEvent);
}
```

### 2. Keep Events Immutable

Make event properties read-only or use init-only setters:

```csharp
public class PlayerDamagedEvent : IEvent
{
    public string EventName => "PlayerDamaged";
    public int Damage { get; init; }
    public Vector3 HitPosition { get; init; }
}
```

### 3. Use Descriptive Event Names

Make event names clear and consistent:

```csharp
// Good
public class PlayerHealthChangedEvent : IEvent
{
    public string EventName => "PlayerHealthChanged";
}

// Avoid
public class Event1 : IEvent
{
    public string EventName => "E1";
}
```

### 4. Batch Event Processing

Process events at predictable times:

```csharp
private void LateUpdate()
{
    // Process all events at end of frame
    _eventQueue.DispatchEvents();
}
```

### 5. Consider Event Priority

For complex systems, queue events in order of importance:

```csharp
// Queue critical events first
_eventQueue.QueueEvent(new GameOverEvent());

// Then queue less critical events
_eventQueue.QueueEvent(new ScoreUpdatedEvent());
```

### 6. Error Handling

The EventQueue handles exceptions during dispatch, but log important errors:

```csharp
private void OnMyEvent(MyEvent evt)
{
    try
    {
        // Event handling logic
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error handling {evt.EventName}: {ex.Message}");
        throw; // Re-throw if critical
    }
}
```

## Common Use Cases

### Game State Transitions

```csharp
public class StateTransitionEvent : IEvent
{
    public string EventName => "StateTransition";
    public string FromState { get; init; }
    public string ToState { get; init; }
}

// Queue state transitions
_eventQueue.QueueEvent(new StateTransitionEvent 
{ 
    FromState = "Menu", 
    ToState = "Gameplay" 
});
```

### Audio Triggering

```csharp
public class PlaySoundEvent : IEvent
{
    public string EventName => "PlaySound";
    public string SoundName { get; init; }
    public float Volume { get; init; }
}

// Audio service listens for sound events
public class AudioService : IAudioService
{
    public AudioService(IEventQueue eventQueue)
    {
        eventQueue.Subscribe<PlaySoundEvent>(OnPlaySound);
    }
    
    private void OnPlaySound(PlaySoundEvent evt)
    {
        // Play the sound
    }
}
```

### Achievement System

```csharp
public class AchievementUnlockedEvent : IEvent
{
    public string EventName => "AchievementUnlocked";
    public string AchievementId { get; init; }
    public string Title { get; init; }
}

public class UIService
{
    public UIService(IEventQueue eventQueue)
    {
        eventQueue.Subscribe<AchievementUnlockedEvent>(OnAchievementUnlocked);
    }
    
    private void OnAchievementUnlocked(AchievementUnlockedEvent evt)
    {
        ShowAchievementNotification(evt.Title);
    }
}
```

### Combat System

```csharp
public class DamageDealtEvent : IEvent
{
    public string EventName => "DamageDealt";
    public GameObject Attacker { get; init; }
    public GameObject Target { get; init; }
    public int Damage { get; init; }
}

public class CombatLogService
{
    public CombatLogService(IEventQueue eventQueue)
    {
        eventQueue.Subscribe<DamageDealtEvent>(OnDamageDealt);
    }
    
    private void OnDamageDealt(DamageDealtEvent evt)
    {
        LogCombatAction($"{evt.Attacker.name} dealt {evt.Damage} damage to {evt.Target.name}");
    }
}
```

## Performance Considerations

### Memory Allocation

- EventQueue pre-allocates collections (64 events, 32 handlers)
- Minimal allocations during normal operation
- Events themselves may allocate (keep them small)

### Processing Time

- O(1) for queueing events
- O(n) for dispatching n events
- O(1) for handler lookup per event type

### Optimization Tips

1. **Limit Event Data**: Keep events lightweight
2. **Batch Processing**: Dispatch events once per frame
3. **Unsubscribe Unused Handlers**: Clean up when no longer needed
4. **Avoid Deep Event Chains**: Limit cascading events

## Testing EventQueue

### Unit Testing Example

```csharp
[Test]
public void EventQueue_QueuesAndDispatchesEvents()
{
    // Arrange
    var eventQueue = new EventQueue();
    bool eventReceived = false;
    
    eventQueue.Subscribe<TestEvent>(evt => eventReceived = true);
    
    // Act
    eventQueue.QueueEvent(new TestEvent());
    eventQueue.DispatchEvents();
    
    // Assert
    Assert.IsTrue(eventReceived);
}
```

## Troubleshooting

### Events Not Being Received

- Verify subscription before event is queued
- Check event type matches subscription type
- Ensure DispatchEvents() is called

### Memory Leaks

- Always unsubscribe in OnDestroy or OnDisable
- Use weak references for long-lived subscribers if needed

### Event Order Issues

- Remember FIFO ordering
- Consider using timestamps if order matters
- Dispatch events at consistent points in game loop

## Migration Guide

### From Unity Events

Before:
```csharp
public UnityEvent<int> OnScoreChanged;
OnScoreChanged?.Invoke(newScore);
```

After:
```csharp
_eventQueue.QueueEvent(new ScoreChangedEvent { NewScore = newScore });
```

### From Message Bus

The EventQueue is similar to a message bus but with explicit dispatch control:

```csharp
// Message Bus (immediate)
messageBus.Publish(new MyEvent());

// EventQueue (controlled)
eventQueue.QueueEvent(new MyEvent());
eventQueue.DispatchEvents(); // Call when ready
```

## See Also

- [README.md](README.md) - Main project documentation
- [ARCHITECTURE.md](ARCHITECTURE.md) - System architecture overview
- [EventQueueExample.cs](Assets/Scripts/Examples/EventQueueExample.cs) - Basic usage example
- [EventQueueIoCExample.cs](Assets/Scripts/Examples/EventQueueIoCExample.cs) - IoC integration example
