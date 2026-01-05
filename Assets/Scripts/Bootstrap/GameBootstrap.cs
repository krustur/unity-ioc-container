using UnityEngine;
using UnityIoC.GameStates;

namespace UnityIoC.Bootstrap
{
    /// <summary>
    /// Bootstrap MonoBehaviour that initializes the IoC container and starts the game.
    /// Attach this to a GameObject in your initial scene.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Bootstrap Settings")]
        [SerializeField]
        [Tooltip("The initial state to transition to on startup")]
        private StartupState _startupState = StartupState.Menu;
        
        [Header("Configuration Objects")]
        [SerializeField]
        [Tooltip("ScriptableObject configurations to register as singletons in the IoC container")]
        private GameConfiguration[] _configurations = new GameConfiguration[0];
        
        private IContainer _container;
        private IGameStateManager _stateManager;
        
        private void Awake()
        {
            // Prevent this GameObject from being destroyed on scene load
            DontDestroyOnLoad(gameObject);
            
            // Initialize the IoC container
            InitializeContainer();
            
            // Start the game
            StartGame();
        }
        
        private void Update()
        {
            // Update the current game state
            _stateManager?.Update();
        }
        
        /// <summary>
        /// Initializes the IoC container and registers all dependencies.
        /// </summary>
        private void InitializeContainer()
        {
            Debug.Log("Initializing IoC Container...");
            
            // Create the container
            _container = new Container();
            
            // Register the container itself (for services that need it)
            _container.RegisterInstance<IContainer>(_container);
            
            // Register configuration ScriptableObjects as singletons
            RegisterConfigurations();
            
            // Register core services as singletons
            _container.Register<IGameStateManager, GameStateManager>(ServiceLifetime.Singleton);
            
            // Register game states as transient (new instance per transition)
            _container.Register<GameMenuState, GameMenuState>(ServiceLifetime.Transient);
            _container.Register<GameEditorState, GameEditorState>(ServiceLifetime.Transient);
            _container.Register<GamePlayState, GamePlayState>(ServiceLifetime.Transient);
            
            // TODO: Register additional game services here
            // Example:
            // _container.Register<IAudioService, AudioService>(ServiceLifetime.Singleton);
            // _container.Register<IInputService, InputService>(ServiceLifetime.Singleton);
            // _container.Register<ISaveService, SaveService>(ServiceLifetime.Singleton);
            
            Debug.Log("IoC Container initialized successfully.");
        }
        
        /// <summary>
        /// Registers all configuration ScriptableObjects in the IoC container as singletons.
        /// Each configuration is registered by its concrete type, allowing services to inject specific configurations.
        /// </summary>
        private void RegisterConfigurations()
        {
            if (_configurations == null || _configurations.Length == 0)
            {
                Debug.Log("No configuration objects to register.");
                return;
            }
            
            Debug.Log($"Registering {_configurations.Length} configuration object(s)...");
            
            foreach (var config in _configurations)
            {
                if (config == null)
                {
                    Debug.LogWarning("Null configuration found in array, skipping.");
                    continue;
                }
                
                // Register the configuration by its concrete type
                var configType = config.GetType();
                var registerMethod = typeof(IContainer).GetMethod(nameof(IContainer.RegisterInstance));
                var genericMethod = registerMethod.MakeGenericMethod(configType);
                genericMethod.Invoke(_container, new object[] { config });
                
                // Call the OnRegistered callback
                config.OnRegistered();
            }
        }
        
        /// <summary>
        /// Starts the game by transitioning to the initial state.
        /// </summary>
        private void StartGame()
        {
            Debug.Log("Starting game...");
            
            // Resolve the game state manager
            _stateManager = _container.Resolve<IGameStateManager>();
            
            // Transition to the initial state based on settings
            switch (_startupState)
            {
                case StartupState.Menu:
                    _stateManager.TransitionTo<GameMenuState>();
                    break;
                    
                case StartupState.Editor:
                    _stateManager.TransitionTo<GameEditorState>();
                    break;
                    
                case StartupState.Game:
                    _stateManager.TransitionTo<GamePlayState>();
                    break;
            }
            
            Debug.Log("Game started successfully.");
        }
        
        private void OnDestroy()
        {
            Debug.Log("GameBootstrap destroyed.");
        }
        
        /// <summary>
        /// Defines the possible startup states.
        /// </summary>
        private enum StartupState
        {
            Menu,
            Editor,
            Game
        }
    }
}
