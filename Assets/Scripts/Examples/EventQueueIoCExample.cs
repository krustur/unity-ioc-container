using UnityEngine;
using UnityIoC.EventQueue;
using UnityIoC.GameStates;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example demonstrating EventQueue integration with the IoC container.
    /// This shows how to register the EventQueue as a singleton service and use it across different services.
    /// </summary>
    public class EventQueueIoCExample : MonoBehaviour
    {
        private IContainer _container;
        private IGameEventService _gameEventService;
        private IScoreService _scoreService;
        
        private void Awake()
        {
            Debug.Log("=== EventQueue IoC Integration Example ===");
            
            // Initialize container
            _container = new Container();
            _container.RegisterInstance<IContainer>(_container);
            
            // Register EventQueue as a singleton
            _container.Register<IEventQueue, EventQueue.EventQueue>(ServiceLifetime.Singleton);
            
            // Register example services
            _container.Register<IGameEventService, GameEventService>(ServiceLifetime.Singleton);
            _container.Register<IScoreService, ScoreService>(ServiceLifetime.Singleton);
            
            // Resolve services
            _gameEventService = _container.Resolve<IGameEventService>();
            _scoreService = _container.Resolve<IScoreService>();
            
            Debug.Log("Services initialized. Press SPACE to trigger game events.");
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SimulateGameplay();
            }
        }
        
        private void SimulateGameplay()
        {
            Debug.Log("\n--- Simulating Gameplay ---");
            
            // Queue multiple events
            _gameEventService.TriggerItemCollection("Gold Coin", 50);
            _gameEventService.TriggerItemCollection("Ruby", 100);
            _gameEventService.TriggerEnemyDefeated("Goblin", 25);
            
            // Process all events
            _gameEventService.ProcessEvents();
            
            // Display score
            Debug.Log($"Final Score: {_scoreService.GetScore()}, Final XP: {_scoreService.GetExperience()}");
        }
        
        private void OnDestroy()
        {
            _gameEventService?.Cleanup();
        }
    }
    
    /// <summary>
    /// Interface for game event service.
    /// </summary>
    public interface IGameEventService
    {
        void TriggerItemCollection(string itemName, int points);
        void TriggerEnemyDefeated(string enemyType, int experience);
        void ProcessEvents();
        void Cleanup();
    }
    
    /// <summary>
    /// Service that manages game events using the EventQueue.
    /// </summary>
    public class GameEventService : IGameEventService
    {
        private readonly IEventQueue _eventQueue;
        
        public GameEventService(IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
            Debug.Log("GameEventService initialized with EventQueue dependency.");
        }
        
        public void TriggerItemCollection(string itemName, int points)
        {
            var evt = new ItemCollectedEvent(itemName, points);
            _eventQueue.QueueEvent(evt);
            Debug.Log($"Event queued: Item '{itemName}' collected");
        }
        
        public void TriggerEnemyDefeated(string enemyType, int experience)
        {
            var evt = new EnemyDefeatedEvent(enemyType, experience);
            _eventQueue.QueueEvent(evt);
            Debug.Log($"Event queued: Enemy '{enemyType}' defeated");
        }
        
        public void ProcessEvents()
        {
            Debug.Log($"Processing {_eventQueue.Count} events...");
            _eventQueue.DispatchEvents();
        }
        
        public void Cleanup()
        {
            _eventQueue.Clear();
        }
    }
    
    /// <summary>
    /// Interface for score tracking service.
    /// </summary>
    public interface IScoreService
    {
        int GetScore();
        int GetExperience();
    }
    
    /// <summary>
    /// Service that tracks player score and experience by listening to events.
    /// </summary>
    public class ScoreService : IScoreService
    {
        private readonly IEventQueue _eventQueue;
        private int _score;
        private int _experience;
        
        public ScoreService(IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
            
            // Subscribe to events
            _eventQueue.Subscribe<ItemCollectedEvent>(OnItemCollected);
            _eventQueue.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
            
            Debug.Log("ScoreService initialized and subscribed to events.");
        }
        
        public int GetScore() => _score;
        public int GetExperience() => _experience;
        
        private void OnItemCollected(ItemCollectedEvent evt)
        {
            _score += evt.Points;
            Debug.Log($"[ScoreService] Score increased by {evt.Points}. Total: {_score}");
        }
        
        private void OnEnemyDefeated(EnemyDefeatedEvent evt)
        {
            _experience += evt.ExperienceGained;
            Debug.Log($"[ScoreService] Experience increased by {evt.ExperienceGained}. Total: {_experience}");
        }
    }
}
