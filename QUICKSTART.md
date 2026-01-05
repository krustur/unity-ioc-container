# Quick Start Guide

## Installation
1. Copy the `Assets` folder into your Unity 6.0 project
2. Open Unity Editor

## Setup
1. Create a new scene (or use existing)
2. Create an empty GameObject and name it "GameBootstrap"
3. Add the `GameBootstrap` component to it
4. In the Inspector, select your preferred startup state:
   - **Menu**: Start in the main menu (default)
   - **Editor**: Start in the level editor
   - **Game**: Start directly in gameplay

## Test the Implementation
1. Press Play in Unity Editor
2. Watch the Console for state transition messages
3. Test keyboard controls:
   - In Menu: Press `Enter` or `Space` to start game, `E` for editor
   - In Editor: Press `Escape` for menu, `P` to test gameplay
   - In Game: Press `Escape` to return to menu

## Adding Your Own Services

### 1. Define Your Service Interface
```csharp
public interface IMyService
{
    void DoSomething();
}
```

### 2. Implement the Service
```csharp
public class MyService : IMyService
{
    public void DoSomething()
    {
        Debug.Log("Doing something!");
    }
}
```

### 3. Register in GameBootstrap
Edit `Assets/Scripts/Bootstrap/GameBootstrap.cs`, in the `InitializeContainer()` method:
```csharp
// Add after existing registrations
_container.Register<IMyService, MyService>(ServiceLifetime.Singleton);
```

### 4. Use in Your Game States
```csharp
public class MyCustomState : IGameState
{
    private readonly IMyService _myService;
    
    // Constructor injection
    public MyCustomState(IMyService myService)
    {
        _myService = myService;
    }
    
    public void Enter()
    {
        _myService.DoSomething();
    }
    
    public void Update() { }
    public void Exit() { }
}
```

### 5. Register Your State
In `GameBootstrap.InitializeContainer()`:
```csharp
_container.Register<MyCustomState, MyCustomState>(ServiceLifetime.Transient);
```

## Service Lifetimes

### Singleton
- One instance shared across the entire game
- Use for: Audio, Input, Save/Load, Settings
```csharp
_container.Register<IAudioService, AudioService>(ServiceLifetime.Singleton);
```

### Transient
- New instance created every time
- Use for: Game states, temporary objects, factories
```csharp
_container.Register<IEnemyFactory, EnemyFactory>(ServiceLifetime.Transient);
```

## Project Structure
```
Assets/
├── Scripts/
│   ├── IoC/                    # Core container
│   ├── Bootstrap/              # Game initialization
│   ├── GameStates/             # State management
│   └── Examples/               # Usage examples
└── Scenes/                     # Your game scenes
```

## Next Steps
1. Replace placeholder states with your actual game logic
2. Add your services (Audio, Input, UI, etc.)
3. Create additional game states as needed
4. Build your game!

## Common Patterns

### Factory Pattern
```csharp
public interface IEnemyFactory
{
    Enemy Create(EnemyType type);
}

_container.Register<IEnemyFactory>(c => 
    new EnemyFactory(c.Resolve<IAudioService>()), 
    ServiceLifetime.Singleton);
```

### Conditional Registration
```csharp
#if UNITY_EDITOR
_container.Register<IDebugService, DebugService>(ServiceLifetime.Singleton);
#else
_container.Register<IDebugService, NullDebugService>(ServiceLifetime.Singleton);
#endif
```

### Instance Registration
```csharp
var config = new GameConfig { Difficulty = 5 };
_container.RegisterInstance<GameConfig>(config);
```

## Troubleshooting

### "Service not registered"
- Make sure you registered the service in `GameBootstrap.InitializeContainer()`
- Check the interface name matches exactly

### "Cannot resolve dependency"
- All constructor parameters must be registered in the container
- Register dependencies before the services that need them

### State doesn't change
- Make sure you called `_stateManager.TransitionTo<YourState>()`
- Verify your state is registered in the container

## Performance Tips
1. Use Singleton for services you only need one instance of
2. Pre-register all services at startup (avoid runtime registration)
3. Keep state Update() methods efficient
4. Use the container for dependency injection, not service location

## Need Help?
See the full README.md for detailed documentation and examples.
