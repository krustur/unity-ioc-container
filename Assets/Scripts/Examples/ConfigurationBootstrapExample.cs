using UnityEngine;
using UnityIoC.Bootstrap;
using UnityIoC.GameStates;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Extended GameBootstrap example demonstrating configuration usage.
    /// This shows how to set up the IoC container with configuration objects
    /// and custom services that use those configurations.
    /// 
    /// USAGE INSTRUCTIONS:
    /// 1. Create configuration assets:
    ///    - Right-click in Project window
    ///    - Select "Create > Game Configuration > Audio Configuration"
    ///    - Select "Create > Game Configuration > Gameplay Configuration"
    ///    - Configure the values in the Inspector
    /// 
    /// 2. Set up the bootstrap:
    ///    - Create an empty GameObject in your scene
    ///    - Attach this ConfigurationBootstrapExample component
    ///    - Drag your configuration assets to the "Configurations" array in the Inspector
    /// 
    /// 3. Run the scene:
    ///    - Configurations will be automatically registered as singletons
    ///    - Services can inject and use the configurations
    ///    - Check the Console for logs showing configuration values being used
    /// </summary>
    public class ConfigurationBootstrapExample : MonoBehaviour
    {
        [Header("Bootstrap Settings")]
        [SerializeField]
        [Tooltip("The initial state to transition to on startup")]
        private StartupState _startupState = StartupState.Menu;
        
        [Header("Configuration Objects")]
        [SerializeField]
        [Tooltip("ScriptableObject configurations to register as singletons")]
        private GameConfiguration[] _configurations = { };
        
        private IContainer _container;
        private IGameStateManager _stateManager;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeContainer();
            StartGame();
        }
        
        private void Update()
        {
            _stateManager?.Update();
        }
        
        /// <summary>
        /// Initializes the IoC container with configurations and services.
        /// </summary>
        private void InitializeContainer()
        {
            Debug.Log("=== Initializing IoC Container with Configurations ===");
            
            _container = new Container();
            _container.RegisterInstance<IContainer>(_container);
            
            // Register configurations first
            _container.RegisterConfigurations(_configurations);
            
            // Register services that depend on configurations
            // Note: These services can inject the configuration objects in their constructors
            _container.Register<IAudioService, ConfigurableAudioService>(ServiceLifetime.Singleton);
            _container.Register<IGameplayService, GameplayService>(ServiceLifetime.Singleton);
            
            // Register state management
            _container.Register<IGameStateManager, GameStateManager>(ServiceLifetime.Singleton);
            _container.Register<GameMenuState, GameMenuState>(ServiceLifetime.Transient);
            _container.Register<GameEditorState, GameEditorState>(ServiceLifetime.Transient);
            _container.Register<GamePlayState, GamePlayState>(ServiceLifetime.Transient);
            _container.Register<ConfiguredGameState, ConfiguredGameState>(ServiceLifetime.Transient);
            
            Debug.Log("=== IoC Container Initialized Successfully ===");
        }
        
        /// <summary>
        /// Starts the game and demonstrates configuration usage.
        /// </summary>
        private void StartGame()
        {
            Debug.Log("=== Starting Game with Configuration Demo ===");
            
            // Resolve and test services that use configurations
            DemonstrateConfigurationUsage();
            
            // Start state management
            _stateManager = _container.Resolve<IGameStateManager>();
            
            switch (_startupState)
            {
                case StartupState.Menu:
                    _stateManager.TransitionTo<GameMenuState>();
                    break;
                case StartupState.ConfiguredGame:
                    _stateManager.TransitionTo<ConfiguredGameState>();
                    break;
            }
            
            Debug.Log("=== Game Started Successfully ===");
        }
        
        /// <summary>
        /// Demonstrates how services use injected configurations.
        /// </summary>
        private void DemonstrateConfigurationUsage()
        {
            Debug.Log("--- Configuration Usage Demo ---");
            
            // Resolve services (they will automatically inject configurations)
            if (_container.IsRegistered<IAudioService>())
            {
                var audioService = _container.Resolve<IAudioService>();
                audioService.PlayMusic("MenuTheme");
                audioService.PlaySound("ButtonClick");
            }
            
            if (_container.IsRegistered<IGameplayService>())
            {
                var gameplayService = _container.Resolve<IGameplayService>();
                gameplayService.InitializePlayer();
                gameplayService.ApplyDifficulty();
                gameplayService.CheckAutoSave();
            }
            
            Debug.Log("--- Demo Complete ---");
        }
        
        private enum StartupState
        {
            Menu,
            ConfiguredGame
        }
    }
    
    /// <summary>
    /// Example game state that uses multiple configurations.
    /// </summary>
    public class ConfiguredGameState : IGameState
    {
        private readonly IGameStateManager _stateManager;
        private readonly IAudioService _audioService;
        private readonly IGameplayService _gameplayService;
        
        public ConfiguredGameState(
            IGameStateManager stateManager,
            IAudioService audioService,
            IGameplayService gameplayService)
        {
            _stateManager = stateManager;
            _audioService = audioService;
            _gameplayService = gameplayService;
        }
        
        public void Enter()
        {
            Debug.Log("=== Entering Configured Game State ===");
            _audioService.PlayMusic("GameplayTheme");
            _gameplayService.InitializePlayer();
            _gameplayService.ApplyDifficulty();
        }
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _stateManager.TransitionTo<GameMenuState>();
            }
        }
        
        public void Exit()
        {
            Debug.Log("=== Exiting Configured Game State ===");
            _audioService.StopMusic();
        }
    }
}
