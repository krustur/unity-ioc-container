using UnityEngine;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example audio configuration ScriptableObject.
    /// Create instances via: Assets > Create > Game Configuration > Audio Configuration
    /// </summary>
    [CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Game Configuration/Audio Configuration", order = 1)]
    public class AudioConfiguration : GameConfiguration
    {
        [Header("Volume Settings")]
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Master volume level (0-1)")]
        private float _masterVolume = 1f;
        
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Music volume level (0-1)")]
        private float _musicVolume = 0.7f;
        
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Sound effects volume level (0-1)")]
        private float _sfxVolume = 1f;
        
        [Header("Audio Settings")]
        [SerializeField]
        [Tooltip("Enable or disable audio")]
        private bool _audioEnabled = true;
        
        [SerializeField]
        [Tooltip("Maximum number of simultaneous sound effects")]
        private int _maxSimultaneousSounds = 32;
        
        // Public properties to access configuration values
        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public bool AudioEnabled => _audioEnabled;
        public int MaxSimultaneousSounds => _maxSimultaneousSounds;
        
        /// <summary>
        /// Gets the effective music volume (master * music).
        /// </summary>
        public float EffectiveMusicVolume => _masterVolume * _musicVolume;
        
        /// <summary>
        /// Gets the effective SFX volume (master * sfx).
        /// </summary>
        public float EffectiveSfxVolume => _masterVolume * _sfxVolume;
    }
}
