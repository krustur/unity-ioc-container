# Unity IoC Container

A high-performance Inversion of Control (IoC) container designed for Unity 6.0 games. This container supports singleton and transient service lifetimes and includes a complete game state management system with bootstrap setup.

## Features

- **High Performance**: Optimized for Unity 6.0 with minimal allocations and fast dictionary-based lookups
- **Service Lifetimes**: Support for both Singleton and Transient lifecycles
- **Constructor Injection**: Automatic dependency resolution through constructor injection
- **Game State Management**: Built-in state management system for game flow
- **Bootstrap System**: Easy setup with MonoBehaviour bootstrap component
- **Flexible Registration**: Support for type registration, factory functions, and instance registration

## Quick Start

### 1. Setup

Copy the `Assets` folder to your Unity 6.0 project.

### 2. Create a Bootstrap Scene

1. Create a new scene in Unity
2. Create an empty GameObject named "GameBootstrap"
3. Attach the `GameBootstrap` component to it
4. Configure the startup state (Menu, Editor, or Game) in the Inspector

### 3. Run Your Game

Press Play in Unity. The IoC container will initialize, register all dependencies, and transition to your chosen initial state.

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
│   └── GameStates/
│       ├── IGameState.cs          # Game state interface
│       ├── IGameStateManager.cs   # State manager interface
│       ├── GameStateManager.cs    # State manager implementation
│       ├── GameMenuState.cs       # Menu state
│       ├── GameEditorState.cs     # Editor state
│       └── GamePlayState.cs       # Gameplay state
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