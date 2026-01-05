using UnityEngine;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example gameplay configuration ScriptableObject.
    /// Create instances via: Assets > Create > Game Configuration > Gameplay Configuration
    /// </summary>
    [CreateAssetMenu(fileName = "GameplayConfiguration", menuName = "Game Configuration/Gameplay Configuration", order = 2)]
    public class GameplayConfiguration : GameConfiguration
    {
        [Header("Player Settings")]
        [SerializeField]
        [Tooltip("Player movement speed")]
        private float _playerSpeed = 5f;
        
        [SerializeField]
        [Tooltip("Player maximum health")]
        private int _playerMaxHealth = 100;
        
        [SerializeField]
        [Tooltip("Player starting lives")]
        private int _playerLives = 3;
        
        [Header("Difficulty Settings")]
        [SerializeField]
        [Tooltip("Game difficulty level")]
        private DifficultyLevel _difficulty = DifficultyLevel.Normal;
        
        [SerializeField]
        [Tooltip("Enable friendly fire")]
        private bool _friendlyFireEnabled = false;
        
        [Header("Game Rules")]
        [SerializeField]
        [Tooltip("Time limit in seconds (0 = no limit)")]
        private float _timeLimitSeconds = 300f;
        
        [SerializeField]
        [Tooltip("Enable auto-save")]
        private bool _autoSaveEnabled = true;
        
        [SerializeField]
        [Tooltip("Auto-save interval in seconds")]
        private float _autoSaveIntervalSeconds = 60f;
        
        // Public properties
        public float PlayerSpeed => _playerSpeed;
        public int PlayerMaxHealth => _playerMaxHealth;
        public int PlayerLives => _playerLives;
        public DifficultyLevel Difficulty => _difficulty;
        public bool FriendlyFireEnabled => _friendlyFireEnabled;
        public float TimeLimitSeconds => _timeLimitSeconds;
        public bool AutoSaveEnabled => _autoSaveEnabled;
        public float AutoSaveIntervalSeconds => _autoSaveIntervalSeconds;
        
        /// <summary>
        /// Gets the difficulty multiplier for various game calculations.
        /// </summary>
        public float DifficultyMultiplier
        {
            get
            {
                return _difficulty switch
                {
                    DifficultyLevel.Easy => 0.75f,
                    DifficultyLevel.Normal => 1.0f,
                    DifficultyLevel.Hard => 1.5f,
                    DifficultyLevel.Expert => 2.0f,
                    _ => 1.0f
                };
            }
        }
    }
    
    /// <summary>
    /// Difficulty levels for the game.
    /// </summary>
    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard,
        Expert
    }
}
