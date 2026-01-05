# Configuration System Guide

This guide explains how to use ScriptableObject-based configuration with the Unity IoC Container.

## Overview

The configuration system allows you to externalize game settings into ScriptableObject assets that can be:
- Created and edited in the Unity Editor
- Registered as singletons in the IoC container
- Injected into services and game states automatically
- Modified without changing code

## Quick Start

### 1. Create Configuration Assets

Right-click in your Project window and navigate to:
- **Create > Game Configuration > Audio Configuration**
- **Create > Game Configuration > Gameplay Configuration**

Configure the values in the Inspector to match your game's requirements.

### 2. Attach Configurations to GameBootstrap

1. Open the scene with your GameBootstrap GameObject
2. Select the GameBootstrap GameObject
3. In the Inspector, find the **"Configuration Objects"** array
4. Increase the array size and drag your configuration assets into the slots

### 3. Use Configurations in Your Services

Your services can now inject configurations through their constructors:

```csharp
public class MyAudioService : IAudioService
{
    private readonly AudioConfiguration _config;
    
    // Configuration is automatically injected
    public MyAudioService(AudioConfiguration config)
    {
        _config = config;
    }
    
    public void PlaySound(string soundName)
    {
        float volume = _config.EffectiveSfxVolume;
        // Play sound at configured volume...
    }
}
```

## Creating Custom Configurations

### Step 1: Create a Configuration Class

Create a new C# script that inherits from `GameConfiguration`:

```csharp
using UnityEngine;
using UnityIoC;

[CreateAssetMenu(fileName = "MyConfiguration", menuName = "Game Configuration/My Configuration")]
public class MyConfiguration : GameConfiguration
{
    [Header("My Settings")]
    [SerializeField]
    private float _mySetting = 1.0f;
    
    public float MySetting => _mySetting;
    
    public override void OnRegistered()
    {
        base.OnRegistered();
        Debug.Log($"MyConfiguration registered with MySetting: {_mySetting}");
    }
}
```

### Step 2: Create an Asset

1. Right-click in Project window
2. Navigate to **Create > Game Configuration > My Configuration**
3. Name your asset and configure values in Inspector

### Step 3: Add to GameBootstrap

1. Select your GameBootstrap GameObject
2. Add the new configuration asset to the **Configurations** array

### Step 4: Use in Services

Inject the configuration into any service:

```csharp
public class MyService
{
    private readonly MyConfiguration _config;
    
    public MyService(MyConfiguration config)
    {
        _config = config;
        Debug.Log($"Service initialized with setting: {_config.MySetting}");
    }
}
```

## Example Configurations Provided

### AudioConfiguration

Controls audio settings like volume levels and enabled state.

**Properties:**
- `MasterVolume` - Overall volume (0-1)
- `MusicVolume` - Music volume (0-1)
- `SfxVolume` - Sound effects volume (0-1)
- `AudioEnabled` - Enable/disable audio
- `MaxSimultaneousSounds` - Max concurrent sounds
- `EffectiveMusicVolume` - Calculated (master * music)
- `EffectiveSfxVolume` - Calculated (master * sfx)

**Usage:**
```csharp
public class AudioService : IAudioService
{
    private readonly AudioConfiguration _config;
    
    public AudioService(AudioConfiguration config)
    {
        _config = config;
    }
    
    public void PlayMusic(string name)
    {
        if (!_config.AudioEnabled) return;
        // Use _config.EffectiveMusicVolume...
    }
}
```

### GameplayConfiguration

Controls gameplay settings like difficulty and player stats.

**Properties:**
- `PlayerSpeed` - Player movement speed
- `PlayerMaxHealth` - Player health maximum
- `PlayerLives` - Starting lives
- `Difficulty` - Difficulty level enum
- `FriendlyFireEnabled` - Enable friendly fire
- `TimeLimitSeconds` - Time limit (0 = none)
- `AutoSaveEnabled` - Enable auto-save
- `AutoSaveIntervalSeconds` - Auto-save frequency
- `DifficultyMultiplier` - Calculated multiplier (0.75-2.0)

**Usage:**
```csharp
public class GameplayService : IGameplayService
{
    private readonly GameplayConfiguration _config;
    
    public GameplayService(GameplayConfiguration config)
    {
        _config = config;
    }
    
    public void InitializePlayer()
    {
        float speed = _config.PlayerSpeed;
        int health = _config.PlayerMaxHealth;
        // Setup player...
    }
}
```

## Complete Example

See `ConfigurationBootstrapExample.cs` for a complete working example that demonstrates:
- Creating and registering configurations
- Services that inject configurations
- Game states that use configured services

To try the example:
1. Create AudioConfiguration and GameplayConfiguration assets
2. Create a GameObject with `ConfigurationBootstrapExample` component
3. Assign your configuration assets to the array
4. Run the scene and watch the Console logs

## Best Practices

1. **Create Different Configurations for Different Environments**
   - DevelopmentConfig, ProductionConfig, TestingConfig
   - Swap them in GameBootstrap for different builds

2. **Use Computed Properties**
   - Calculate derived values (like EffectiveMusicVolume)
   - Keep configuration data simple

3. **Validate Configuration Values**
   - Use `OnRegistered()` to validate settings
   - Log warnings for invalid configurations

4. **Organize Configurations by Domain**
   - AudioConfiguration, GraphicsConfiguration, InputConfiguration
   - Keep configurations focused and cohesive

5. **Version Control Friendly**
   - ScriptableObjects work well with version control
   - Team members can have different local configurations

## Advanced: Runtime Configuration

Configurations registered as ScriptableObjects are immutable at runtime. If you need runtime modification:

```csharp
// Create a mutable wrapper service
public class RuntimeAudioSettings
{
    private readonly AudioConfiguration _baseConfig;
    public float RuntimeMasterVolume { get; set; }
    
    public RuntimeAudioSettings(AudioConfiguration baseConfig)
    {
        _baseConfig = baseConfig;
        RuntimeMasterVolume = _baseConfig.MasterVolume;
    }
    
    public float EffectiveMusicVolume => 
        RuntimeMasterVolume * _baseConfig.MusicVolume;
}
```

## Troubleshooting

**Problem: Service can't resolve configuration**
- Ensure the configuration is added to GameBootstrap's array
- Check that the configuration type matches exactly (e.g., `AudioConfiguration`)
- Verify GameBootstrap runs before the service is resolved

**Problem: Configuration changes not reflected**
- ScriptableObject changes in Inspector require Play mode restart
- For runtime changes, use a mutable wrapper service

**Problem: Null reference exception**
- Check that all configuration slots in the array are assigned
- Verify no null entries in the configurations array

## API Reference

### GameConfiguration Base Class

```csharp
public abstract class GameConfiguration : ScriptableObject
{
    // Override to customize configuration name
    public virtual string ConfigurationName { get; }
    
    // Called when registered in container
    public virtual void OnRegistered() { }
}
```

### ContainerConfigurationExtensions

```csharp
public static class ContainerConfigurationExtensions
{
    // Register a single configuration
    public static void RegisterConfiguration(
        this IContainer container, 
        GameConfiguration configuration);
    
    // Register multiple configurations
    public static void RegisterConfigurations(
        this IContainer container, 
        GameConfiguration[] configurations);
}
```

### GameBootstrap Configuration Support

```csharp
public class GameBootstrap : MonoBehaviour
{
    // Array of configurations to register
    [SerializeField]
    private GameConfiguration[] _configurations;
    
    // Automatically registers all configurations as singletons
    // Uses extension method for optimized registration
    private void RegisterConfigurations() { }
}
```

## Next Steps

- Create your own domain-specific configurations
- Build services that consume configurations
- Set up different configuration profiles for testing
- Integrate with Unity's build system for environment-specific configs
