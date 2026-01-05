using UnityEngine;
using UnityIoC;
using UnityIoC.Examples;

namespace UnityIoC.Tests
{
    /// <summary>
    /// Manual verification script for testing configuration system.
    /// Attach to a GameObject and run to verify configuration registration and injection.
    /// </summary>
    public class ConfigurationSystemVerification : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField]
        [Tooltip("Audio configuration asset for testing")]
        private AudioConfiguration _testAudioConfig;
        
        [SerializeField]
        [Tooltip("Gameplay configuration asset for testing")]
        private GameplayConfiguration _testGameplayConfig;
        
        private void Start()
        {
            Debug.Log("=== Starting Configuration System Verification ===");
            
            // Test 1: Base configuration class
            TestBaseConfiguration();
            
            // Test 2: Configuration registration
            TestConfigurationRegistration();
            
            // Test 3: Service injection
            TestServiceInjection();
            
            Debug.Log("=== Configuration System Verification Complete ===");
        }
        
        private void TestBaseConfiguration()
        {
            Debug.Log("Test 1: Base Configuration Class");
            
            if (_testAudioConfig != null)
            {
                Debug.Log($"✓ AudioConfiguration loaded: {_testAudioConfig.name}");
                Debug.Log($"  - Master Volume: {_testAudioConfig.MasterVolume}");
                Debug.Log($"  - Music Volume: {_testAudioConfig.MusicVolume}");
                Debug.Log($"  - Effective Music Volume: {_testAudioConfig.EffectiveMusicVolume}");
            }
            else
            {
                Debug.LogWarning("✗ No AudioConfiguration assigned for testing");
            }
            
            if (_testGameplayConfig != null)
            {
                Debug.Log($"✓ GameplayConfiguration loaded: {_testGameplayConfig.name}");
                Debug.Log($"  - Player Speed: {_testGameplayConfig.PlayerSpeed}");
                Debug.Log($"  - Difficulty: {_testGameplayConfig.Difficulty}");
                Debug.Log($"  - Difficulty Multiplier: {_testGameplayConfig.DifficultyMultiplier}");
            }
            else
            {
                Debug.LogWarning("✗ No GameplayConfiguration assigned for testing");
            }
        }
        
        private void TestConfigurationRegistration()
        {
            Debug.Log("\nTest 2: Configuration Registration");
            
            var container = new Container();
            
            // Test registering audio configuration
            if (_testAudioConfig != null)
            {
                container.RegisterInstance<AudioConfiguration>(_testAudioConfig);
                
                if (container.IsRegistered<AudioConfiguration>())
                {
                    Debug.Log("✓ AudioConfiguration registered successfully");
                    
                    var resolved = container.Resolve<AudioConfiguration>();
                    if (resolved == _testAudioConfig)
                    {
                        Debug.Log("✓ AudioConfiguration resolved correctly (same instance)");
                    }
                    else
                    {
                        Debug.LogError("✗ AudioConfiguration resolved but not the same instance");
                    }
                }
                else
                {
                    Debug.LogError("✗ AudioConfiguration not registered");
                }
            }
            
            // Test registering gameplay configuration
            if (_testGameplayConfig != null)
            {
                container.RegisterInstance<GameplayConfiguration>(_testGameplayConfig);
                
                if (container.IsRegistered<GameplayConfiguration>())
                {
                    Debug.Log("✓ GameplayConfiguration registered successfully");
                    
                    var resolved = container.Resolve<GameplayConfiguration>();
                    if (resolved == _testGameplayConfig)
                    {
                        Debug.Log("✓ GameplayConfiguration resolved correctly (same instance)");
                    }
                    else
                    {
                        Debug.LogError("✗ GameplayConfiguration resolved but not the same instance");
                    }
                }
                else
                {
                    Debug.LogError("✗ GameplayConfiguration not registered");
                }
            }
        }
        
        private void TestServiceInjection()
        {
            Debug.Log("\nTest 3: Service Injection");
            
            var container = new Container();
            
            // Register configurations
            if (_testAudioConfig != null)
            {
                container.RegisterInstance<AudioConfiguration>(_testAudioConfig);
            }
            
            if (_testGameplayConfig != null)
            {
                container.RegisterInstance<GameplayConfiguration>(_testGameplayConfig);
            }
            
            // Register and resolve services
            if (_testAudioConfig != null)
            {
                container.Register<IAudioService, ConfigurableAudioService>(ServiceLifetime.Singleton);
                
                try
                {
                    var audioService = container.Resolve<IAudioService>();
                    Debug.Log("✓ ConfigurableAudioService created with injected configuration");
                    
                    // Test service functionality
                    audioService.PlaySound("TestSound");
                    audioService.PlayMusic("TestMusic");
                    Debug.Log("✓ ConfigurableAudioService methods executed successfully");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"✗ Failed to create ConfigurableAudioService: {e.Message}");
                }
            }
            
            if (_testGameplayConfig != null)
            {
                container.Register<IGameplayService, GameplayService>(ServiceLifetime.Singleton);
                
                try
                {
                    var gameplayService = container.Resolve<IGameplayService>();
                    Debug.Log("✓ GameplayService created with injected configuration");
                    
                    // Test service functionality
                    gameplayService.InitializePlayer();
                    gameplayService.ApplyDifficulty();
                    gameplayService.CheckAutoSave();
                    Debug.Log("✓ GameplayService methods executed successfully");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"✗ Failed to create GameplayService: {e.Message}");
                }
            }
        }
    }
}
