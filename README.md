# Unity IoC Container

A high-performance Inversion of Control (IoC) container designed for Unity 6.0 games. This container supports singleton and transient service lifetimes and includes a complete game state management system with bootstrap setup.

## Features

- **High Performance**: Optimized for Unity 6.0 with minimal allocations and fast dictionary-based lookups
- **Service Lifetimes**: Support for both Singleton and Transient lifecycles
- **Constructor Injection**: Automatic dependency resolution through constructor injection
- **Game State Management**: Built-in state management system for game flow
- **Bootstrap System**: Easy setup with MonoBehaviour bootstrap component
- **Flexible Registration**: Support for type registration, factory functions, and instance registration
- **ScriptableObject Configuration**: Register configuration assets as singletons for externalized game settings
- **EventQueue System**: Sequential event queuing and dispatching for decoupled communication between systems

## Quick Start

### 1. Setup

Copy the `Assets` folder to your Unity 6.0 project.

### 2. Create a Bootstrap Scene

1. Create a new scene in Unity
2. Create an empty GameObject named "GameBootstrap"
3. Attach the `GameBootstrap` component to it
4. Configure the startup state (Menu, Editor, or Game) in the Inspector
5. (Optional) Create and attach ScriptableObject configurations to externalize game settings

### 3. Run Your Game

Press Play in Unity. The IoC container will initialize, register all dependencies, and transition to your chosen initial state.

## EventQueue System

The EventQueue system provides a robust event management solution for decoupled communication between game systems. See [EVENTQUEUE_GUIDE.md](EVENTQUEUE_GUIDE.md) for detailed documentation.

**Quick Example:**
```csharp
// Define an event
public class ItemCollectedEvent : IEvent
{
    public string EventName => "ItemCollected";
    public string ItemName { get; set; }
    public int Points { get; set; }
}

// Register in IoC container
_container.Register<IEventQueue, EventQueue>(ServiceLifetime.Singleton);

// Use in services
public class GameService
{
    private readonly IEventQueue _eventQueue;
    
    public GameService(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
        _eventQueue.Subscribe<ItemCollectedEvent>(OnItemCollected);
    }
    
    private void OnItemCollected(ItemCollectedEvent evt)
    {
        Debug.Log($"Collected {evt.ItemName} for {evt.Points} points");
    }
}
```

Benefits:
- Decouple systems with event-driven communication
- Sequential event processing for deterministic behavior
- Type-safe event handling
- Easy integration with IoC container

## Configuration System

The container supports ScriptableObject-based configuration for externalized game settings. See [CONFIGURATION_GUIDE.md](CONFIGURATION_GUIDE.md) for detailed documentation.

**Quick Example:**
1. Create configuration assets: `Create > Game Configuration > Audio Configuration`
2. Attach to GameBootstrap's "Configurations" array
3. Inject into services via constructor:
```csharp
public class AudioService : IAudioService
{
    private readonly AudioConfiguration _config;
    
    public AudioService(AudioConfiguration config) 
    {
        _config = config;
    }
}
```

Benefits:
- Externalize game settings without code changes
- Version control friendly
- Team-shareable configurations
- Type-safe dependency injection

## Architecture

### Core Components

#### Container
The main IoC container that manages service registration and resolution.

```csharp
// Create a container
IContainer container = new Container();

// Register services
container.Register<IMyService, MyService>(ServiceLifetime.Singleton);
container.Register<ITransientService, TransientService>(ServiceLifetime.Transient);

// Register with factory
container.Register<IConfigService>(c => new ConfigService("config.json"), ServiceLifetime.Singleton);

// Register instance
container.RegisterInstance<IGameSettings>(mySettings);

// Resolve services
var myService = container.Resolve<IMyService>();
```

#### ServiceLifetime
- **Transient**: A new instance is created every time the service is requested
- **Singleton**: A single instance is created and shared across all requests

#### GameConfiguration
Base class for ScriptableObject configurations that can be registered as singletons.

```csharp
[CreateAssetMenu(fileName = "MyConfig", menuName = "Game Configuration/My Config")]
public class MyConfiguration : GameConfiguration
{
    [SerializeField] private float _mySetting = 1.0f;
    public float MySetting => _mySetting;
}

// Register in GameBootstrap by adding to the Configurations array
// Then inject into services:
public class MyService
{
    public MyService(MyConfiguration config) { }
}
```

### Game State Management

The system includes three placeholder game states:

#### GameMenuState
Handles the main menu UI and navigation.
- Press **Enter** or **Space** to start the game
- Press **E** to open the editor

#### GameEditorState
Handles level/content editing functionality.
- Press **Escape** to return to the menu
- Press **P** to test gameplay

#### GamePlayState
Handles active gameplay.
- Press **Escape** to return to the menu

### Adding Custom Services

Register your services in `GameBootstrap.InitializeContainer()`:

```csharp
// Singleton services (shared instance)
_container.Register<IAudioService, AudioService>(ServiceLifetime.Singleton);
_container.Register<IInputService, InputService>(ServiceLifetime.Singleton);
_container.Register<ISaveService, SaveService>(ServiceLifetime.Singleton);

// Transient services (new instance each time)
_container.Register<IEnemyFactory, EnemyFactory>(ServiceLifetime.Transient);
```

### Creating Custom Game States

#### Parameterless States

1. Implement the `IGameState` interface:

```csharp
public class MyCustomState : IGameState
{
    private readonly IGameStateManager _stateManager;
    private readonly IAudioService _audioService;
    
    // Constructor injection
    public MyCustomState(IGameStateManager stateManager, IAudioService audioService)
    {
        _stateManager = stateManager;
        _audioService = audioService;
    }
    
    public void Enter()
    {
        // Initialize state
    }
    
    public void Update()
    {
        // Update state logic
    }
    
    public void Exit()
    {
        // Cleanup state
    }
}
```

2. Register your state in the container:

```csharp
_container.Register<MyCustomState, MyCustomState>(ServiceLifetime.Transient);
```

3. Transition to your state:

```csharp
_stateManager.TransitionTo<MyCustomState>();
```

#### Parameterized States

For states that require parameters on entry, implement the `IGameState<T>` interface:

```csharp
public class GamePlayStateWithLevel : IGameState<int>
{
    private readonly IGameStateManager _stateManager;
    private int _currentLevel;
    
    public GamePlayStateWithLevel(IGameStateManager stateManager)
    {
        _stateManager = stateManager;
    }
    
    // Parameterless Enter (from IGameState) - uses default
    public void Enter()
    {
        Enter(1); // Default to level 1
    }
    
    // Parameterized Enter (from IGameState<int>)
    public void Enter(int levelNumber)
    {
        _currentLevel = levelNumber;
        Debug.Log($"Loading level {_currentLevel}");
        // Initialize with specific level
    }
    
    public void Update()
    {
        // Update state logic
        // Can transition to next level with parameter:
        // _stateManager.TransitionTo<GamePlayStateWithLevel, int>(_currentLevel + 1);
    }
    
    public void Exit()
    {
        // Cleanup state
    }
}
```

Register and transition with parameters:

```csharp
// Register the state
_container.Register<GamePlayStateWithLevel, GamePlayStateWithLevel>(ServiceLifetime.Transient);

// Transition with parameter
_stateManager.TransitionTo<GamePlayStateWithLevel, int>(3); // Load level 3
```

**Benefits of Parameterized States:**
- Strong compile-time type safety for state parameters
- No need for separate initialization methods
- Clean API for passing data between states
- Backward compatible with existing parameterless states

## Performance Considerations

- The container uses pre-allocated dictionaries to minimize allocations
- Singleton instances are cached for fast subsequent access
- Constructor injection is optimized with minimal reflection
- Dictionary-based lookups provide O(1) service resolution

## Example Project Structure

```
Assets/
├── Scripts/
│   ├── Bootstrap/
│   │   └── GameBootstrap.cs       # Main bootstrap component
│   ├── IoC/
│   │   ├── Container.cs           # IoC container implementation
│   │   ├── IContainer.cs          # Container interface
│   │   └── ServiceLifetime.cs     # Service lifetime enum
│   ├── EventQueue/
│   │   ├── IEvent.cs              # Event interface
│   │   ├── IEventQueue.cs         # EventQueue interface
│   │   └── EventQueue.cs          # EventQueue implementation
│   ├── GameStates/
│   │   ├── IGameState.cs          # Game state interface
│   │   ├── IGameStateManager.cs   # State manager interface
│   │   ├── GameStateManager.cs    # State manager implementation
│   │   ├── GameMenuState.cs       # Menu state
│   │   ├── GameEditorState.cs     # Editor state
│   │   └── GamePlayState.cs       # Gameplay state
│   └── Examples/
│       ├── UsageExample.cs        # Basic usage examples
│       ├── EventQueueExample.cs   # EventQueue standalone example
│       └── EventQueueIoCExample.cs # EventQueue IoC integration
└── Scenes/
    └── Bootstrap.unity            # Initial scene with GameBootstrap
```

## Best Practices

1. **Register all dependencies at startup** in `GameBootstrap.InitializeContainer()`
2. **Use Singleton lifetime for services** that maintain state across the game (audio, input, save systems)
3. **Use Transient lifetime for stateless services** or objects that should be created fresh each time
4. **Inject dependencies through constructors** for better testability
5. **Keep game states focused** on a single responsibility
6. **Use the state manager** for all state transitions to ensure proper cleanup

## License

See LICENSE file for details.