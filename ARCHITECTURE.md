# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Unity Game Engine                        │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    GameBootstrap.cs                          │
│  ┌────────────────────────────────────────────────────┐    │
│  │ • Initialize IoC Container                          │    │
│  │ • Register all dependencies                         │    │
│  │ • Start GameStateManager                            │    │
│  │ • Transition to initial state                       │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    IoC Container                             │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Services Dictionary                                 │    │
│  │ ┌──────────────────────────────────────────────┐  │    │
│  │ │ IContainer → Container (Singleton)           │  │    │
│  │ │ IGameStateManager → GameStateManager (S)     │  │    │
│  │ │ GameMenuState → GameMenuState (Transient)    │  │    │
│  │ │ GameEditorState → GameEditorState (T)        │  │    │
│  │ │ GamePlayState → GamePlayState (T)            │  │    │
│  │ │ ... your custom services ...                 │  │    │
│  │ └──────────────────────────────────────────────┘  │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  GameStateManager                            │
│  ┌────────────────────────────────────────────────────┐    │
│  │ Current State: IGameState                           │    │
│  │                                                      │    │
│  │ TransitionTo<T>():                                  │    │
│  │   1. Exit current state                             │    │
│  │   2. Resolve new state from container               │    │
│  │   3. Enter new state                                │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                    ┌─────────┼─────────┐
                    ▼         ▼         ▼
        ┌───────────────┬─────────────┬───────────────┐
        │ GameMenuState │GameEditorSt │ GamePlayState │
        │               │             │               │
        │ Enter()       │ Enter()     │ Enter()       │
        │ Update()      │ Update()    │ Update()      │
        │ Exit()        │ Exit()      │ Exit()        │
        └───────────────┴─────────────┴───────────────┘
```

## Data Flow

### Initialization Flow
```
Unity Start
    │
    ▼
GameBootstrap.Awake()
    │
    ├─► Create Container
    ├─► Register Services
    ├─► Resolve GameStateManager
    └─► TransitionTo<InitialState>
```

### State Transition Flow
```
User Input / Game Event
    │
    ▼
StateManager.TransitionTo<NewState>()
    │
    ├─► CurrentState.Exit()
    │       │
    │       └─► Cleanup resources
    │
    ├─► Container.Resolve<NewState>()
    │       │
    │       ├─► Find constructor
    │       ├─► Resolve dependencies
    │       └─► Create instance
    │
    └─► NewState.Enter()
            │
            └─► Initialize resources
```

### Service Resolution Flow
```
Container.Resolve<IService>()
    │
    ├─► Check if registered
    │       │
    │       └─[No]─► Throw Exception
    │
    ├─► Check if Singleton
    │       │
    │       └─[Yes]─► Return cached instance
    │
    ├─► Create new instance
    │       │
    │       ├─► Find best constructor
    │       ├─► Resolve dependencies
    │       └─► Activator.CreateInstance()
    │
    └─► [Singleton]─► Cache instance
```

## Component Relationships

```
┌──────────────────────────────────────────────────────────┐
│                   Component Diagram                       │
└──────────────────────────────────────────────────────────┘

    ┌─────────────┐
    │ IContainer  │◄────────────┐
    └─────────────┘             │
           △                     │
           │                     │
           │ implements          │ depends on
           │                     │
    ┌─────────────┐         ┌───────────────────┐
    │  Container  │────────►│ ServiceDescriptor │
    └─────────────┘         └───────────────────┘
                                    │
                                    │ contains
                                    ▼
                            ┌───────────────┐
                            │ServiceLifetime│
                            └───────────────┘

    ┌─────────────┐
    │  IGameState │
    └─────────────┘
           △
           │ implements
           │
    ┌──────┴──────┬────────────┬──────────┐
    │             │            │          │
┌───────┐  ┌──────────┐ ┌─────────┐ ┌────────┐
│ Menu  │  │  Editor  │ │  Play   │ │ Custom │
│ State │  │  State   │ │  State  │ │ State  │
└───────┘  └──────────┘ └─────────┘ └────────┘

    ┌──────────────────┐
    │IGameStateManager │
    └──────────────────┘
           △
           │ implements
           │
    ┌──────────────────┐
    │ GameStateManager │────► IContainer (dependency)
    └──────────────────┘
           │
           │ manages
           ▼
      ┌──────────┐
      │IGameState│
      └──────────┘
```

## Lifecycle

### Container Lifecycle
```
1. Construction → Empty dictionaries created
2. Registration → Services added to dictionary
3. Resolution → Services instantiated on demand
4. Shutdown → Container disposed (automatic)
```

### Game State Lifecycle
```
State Created (Transient)
    │
    ▼
Enter() ──► Initialize resources, setup state
    │
    ▼
Update() ──► [Loop] Process game logic
    │
    ▼
Exit() ──► Cleanup resources, save state
    │
    ▼
State Destroyed
```

### Bootstrap Lifecycle
```
Awake() ──► Initialize container & register services
    │
    ▼
Start() ──► (Not used - setup in Awake)
    │
    ▼
Update() ──► Update current game state
    │
    ▼
OnDestroy() ──► Log cleanup message
```

## Performance Characteristics

### Time Complexity
- Service Registration: O(1)
- Service Resolution (first time): O(1) lookup + O(n) constructor params
- Service Resolution (singleton cached): O(1)
- State Transition: O(1) + state-specific logic

### Space Complexity
- Container: O(n) where n = number of services
- Singleton Cache: O(m) where m = number of singletons
- Per-Transient: O(1)

### Memory Layout
```
Container (1 instance)
├── _services Dictionary<Type, ServiceDescriptor> (~1KB + entries)
└── _singletonInstances Dictionary<Type, object> (~512B + instances)

GameStateManager (1 singleton)
├── _container reference (8 bytes)
└── _currentState reference (8 bytes)

Game States (transient, 0-1 active)
└── Dependencies (injected references, ~8 bytes each)
```

## Extension Points

### 1. Custom Services
```csharp
// Define interface
public interface IMyService { }

// Implement
public class MyService : IMyService { }

// Register in GameBootstrap
_container.Register<IMyService, MyService>(ServiceLifetime.Singleton);
```

### 2. Custom States
```csharp
// Implement IGameState
public class MyState : IGameState { }

// Register
_container.Register<MyState, MyState>(ServiceLifetime.Transient);

// Transition
_stateManager.TransitionTo<MyState>();
```

### 3. Factory Pattern
```csharp
_container.Register<IFactory>(c => 
    new Factory(c.Resolve<IDependency>()), 
    ServiceLifetime.Singleton);
```

## Design Patterns Used

1. **Dependency Injection**: Constructor injection for loose coupling
2. **Service Locator**: Container acts as service registry
3. **State Pattern**: Game state management
4. **Singleton Pattern**: Singleton service lifetime
5. **Factory Pattern**: Factory registration support
6. **Template Method**: IGameState lifecycle methods

## Thread Safety

⚠️ **Note**: This container is NOT thread-safe. It's designed for Unity's single-threaded game loop.

For multi-threaded scenarios:
- Register all services at startup (main thread)
- Resolve all services at startup
- Don't register/resolve during gameplay from worker threads
