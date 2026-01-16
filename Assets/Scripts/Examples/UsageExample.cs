using UnityEngine;
using UnityIoC;
using UnityIoC.GameStates;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example service interface for demonstration purposes.
    /// </summary>
    public interface IAudioService
    {
        void PlaySound(string soundName);
        void PlayMusic(string musicName);
        void StopMusic();
    }
    
    /// <summary>
    /// Example audio service implementation.
    /// </summary>
    public class AudioService : IAudioService
    {
        public void PlaySound(string soundName)
        {
            Debug.Log($"Playing sound: {soundName}");
        }
        
        public void PlayMusic(string musicName)
        {
            Debug.Log($"Playing music: {musicName}");
        }
        
        public void StopMusic()
        {
            Debug.Log("Stopping music");
        }
    }
    
    /// <summary>
    /// Example of a custom game state that uses dependency injection.
    /// </summary>
    public class CustomGameState : IGameState
    {
        private readonly IGameStateManager _stateManager;
        private readonly IAudioService _audioService;
        
        // Dependencies are injected through the constructor
        public CustomGameState(IGameStateManager stateManager, IAudioService audioService)
        {
            _stateManager = stateManager;
            _audioService = audioService;
        }
        
        public void Enter()
        {
            Debug.Log("Entering Custom Game State");
            _audioService.PlayMusic("CustomStateMusic");
        }
        
        public void Update()
        {
            // Custom game logic here
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _stateManager.TransitionTo<GameMenuState>();
            }
        }
        
        public void Exit()
        {
            Debug.Log("Exiting Custom Game State");
            _audioService.StopMusic();
        }
    }
    
    /// <summary>
    /// Example MonoBehaviour showing how to extend GameBootstrap with custom services.
    /// </summary>
    public class ExtendedBootstrapExample : MonoBehaviour
    {
        private void Awake()
        {
            // Create and configure container
            var container = new Container();
            
            // Register core services
            container.RegisterInstance<IContainer>(container);
            container.Register<IGameStateManager, GameStateManager>(ServiceLifetime.Singleton);
            
            // Register custom services
            container.Register<IAudioService, AudioService>(ServiceLifetime.Singleton);
            
            // Register game states (using convenience method for concrete types)
            container.Register<GameMenuState>(ServiceLifetime.Transient);
            container.Register<CustomGameState>(ServiceLifetime.Transient);
            
            // Start the game
            var stateManager = container.Resolve<IGameStateManager>();
            stateManager.TransitionTo<GameMenuState>();
        }
    }
}
