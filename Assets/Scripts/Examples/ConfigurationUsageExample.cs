using UnityEngine;
using UnityIoC;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example service that uses configuration objects from the IoC container.
    /// Demonstrates how to inject and use ScriptableObject configurations.
    /// </summary>
    public class ConfigurableAudioService : IAudioService
    {
        private readonly AudioConfiguration _audioConfig;
        
        /// <summary>
        /// Constructor with dependency injection.
        /// The AudioConfiguration is automatically resolved from the IoC container.
        /// </summary>
        public ConfigurableAudioService(AudioConfiguration audioConfig)
        {
            _audioConfig = audioConfig;
            Debug.Log($"ConfigurableAudioService initialized with configuration: {_audioConfig.name}");
            Debug.Log($"Master Volume: {_audioConfig.MasterVolume}, Music Volume: {_audioConfig.MusicVolume}");
        }
        
        public void PlaySound(string soundName)
        {
            if (!_audioConfig.AudioEnabled)
            {
                Debug.Log($"Audio is disabled. Skipping sound: {soundName}");
                return;
            }
            
            float volume = _audioConfig.EffectiveSfxVolume;
            Debug.Log($"Playing sound '{soundName}' at volume {volume:F2}");
        }
        
        public void PlayMusic(string musicName)
        {
            if (!_audioConfig.AudioEnabled)
            {
                Debug.Log($"Audio is disabled. Skipping music: {musicName}");
                return;
            }
            
            float volume = _audioConfig.EffectiveMusicVolume;
            Debug.Log($"Playing music '{musicName}' at volume {volume:F2}");
        }
        
        public void StopMusic()
        {
            Debug.Log("Stopping music");
        }
    }
    
    /// <summary>
    /// Example gameplay service that uses configuration from the IoC container.
    /// </summary>
    public interface IGameplayService
    {
        void InitializePlayer();
        void ApplyDifficulty();
        void CheckAutoSave();
    }
    
    /// <summary>
    /// Implementation of gameplay service using GameplayConfiguration.
    /// </summary>
    public class GameplayService : IGameplayService
    {
        private readonly GameplayConfiguration _gameplayConfig;
        
        /// <summary>
        /// Constructor with dependency injection.
        /// The GameplayConfiguration is automatically resolved from the IoC container.
        /// </summary>
        public GameplayService(GameplayConfiguration gameplayConfig)
        {
            _gameplayConfig = gameplayConfig;
            Debug.Log($"GameplayService initialized with configuration: {_gameplayConfig.name}");
        }
        
        public void InitializePlayer()
        {
            Debug.Log($"Initializing player with speed: {_gameplayConfig.PlayerSpeed}, " +
                     $"health: {_gameplayConfig.PlayerMaxHealth}, " +
                     $"lives: {_gameplayConfig.PlayerLives}");
        }
        
        public void ApplyDifficulty()
        {
            Debug.Log($"Applying difficulty: {_gameplayConfig.Difficulty} " +
                     $"(multiplier: {_gameplayConfig.DifficultyMultiplier:F2})");
            
            if (_gameplayConfig.FriendlyFireEnabled)
            {
                Debug.Log("Friendly fire is ENABLED");
            }
        }
        
        public void CheckAutoSave()
        {
            if (_gameplayConfig.AutoSaveEnabled)
            {
                Debug.Log($"Auto-save enabled with interval: {_gameplayConfig.AutoSaveIntervalSeconds}s");
            }
            else
            {
                Debug.Log("Auto-save is disabled");
            }
        }
    }
}
