using UnityEngine;
using UnityIoC.EventQueue;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example event representing a player spawning in the game.
    /// </summary>
    public class PlayerSpawnedEvent : IEvent
    {
        public string EventName => "PlayerSpawned";
        public Vector3 Position { get; set; }
        public string PlayerName { get; set; }
        
        public PlayerSpawnedEvent(Vector3 position, string playerName)
        {
            Position = position;
            PlayerName = playerName;
        }
    }
    
    /// <summary>
    /// Example event representing an item being collected.
    /// </summary>
    public class ItemCollectedEvent : IEvent
    {
        public string EventName => "ItemCollected";
        public string ItemName { get; set; }
        public int Points { get; set; }
        
        public ItemCollectedEvent(string itemName, int points)
        {
            ItemName = itemName;
            Points = points;
        }
    }
    
    /// <summary>
    /// Example event representing an enemy being defeated.
    /// </summary>
    public class EnemyDefeatedEvent : IEvent
    {
        public string EventName => "EnemyDefeated";
        public string EnemyType { get; set; }
        public int ExperienceGained { get; set; }
        
        public EnemyDefeatedEvent(string enemyType, int experienceGained)
        {
            EnemyType = enemyType;
            ExperienceGained = experienceGained;
        }
    }
    
    /// <summary>
    /// Example MonoBehaviour demonstrating how to use the EventQueue system.
    /// This example shows queuing events, subscribing to events, and dispatching them.
    /// </summary>
    public class EventQueueExample : MonoBehaviour
    {
        private IEventQueue _eventQueue;
        private int _totalScore;
        private int _totalExperience;
        
        private void Awake()
        {
            // Create an event queue instance
            // In a real game, this would typically be registered in the IoC container
            _eventQueue = new EventQueue.EventQueue();
            
            // Subscribe to events
            _eventQueue.Subscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
            _eventQueue.Subscribe<ItemCollectedEvent>(OnItemCollected);
            _eventQueue.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
            
            Debug.Log("EventQueue Example initialized. Subscribing to events.");
        }
        
        private void Start()
        {
            Debug.Log("=== EventQueue Example Started ===");
            Debug.Log("Press Q to queue a PlayerSpawned event");
            Debug.Log("Press W to queue an ItemCollected event");
            Debug.Log("Press E to queue an EnemyDefeated event");
            Debug.Log("Press D to dispatch all queued events");
            Debug.Log("Press C to clear all queued events");
        }
        
        private void Update()
        {
            // Queue events based on input
            if (Input.GetKeyDown(KeyCode.Q))
            {
                QueuePlayerSpawnedEvent();
            }
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                QueueItemCollectedEvent();
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                QueueEnemyDefeatedEvent();
            }
            
            // Dispatch all queued events
            if (Input.GetKeyDown(KeyCode.D))
            {
                DispatchAllEvents();
            }
            
            // Clear all queued events
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearAllEvents();
            }
        }
        
        private void QueuePlayerSpawnedEvent()
        {
            var randomPosition = new Vector3(
                Random.Range(-10f, 10f),
                0f,
                Random.Range(-10f, 10f)
            );
            
            var evt = new PlayerSpawnedEvent(randomPosition, "Player_" + Random.Range(1, 100));
            _eventQueue.QueueEvent(evt);
            
            Debug.Log($"Queued: PlayerSpawnedEvent at {randomPosition}. Queue count: {_eventQueue.Count}");
        }
        
        private void QueueItemCollectedEvent()
        {
            var items = new[] { "Coin", "Gem", "PowerUp", "HealthPotion" };
            var itemName = items[Random.Range(0, items.Length)];
            var points = Random.Range(10, 100);
            
            var evt = new ItemCollectedEvent(itemName, points);
            _eventQueue.QueueEvent(evt);
            
            Debug.Log($"Queued: ItemCollectedEvent ({itemName}). Queue count: {_eventQueue.Count}");
        }
        
        private void QueueEnemyDefeatedEvent()
        {
            var enemies = new[] { "Goblin", "Orc", "Dragon", "Skeleton" };
            var enemyType = enemies[Random.Range(0, enemies.Length)];
            var experience = Random.Range(50, 200);
            
            var evt = new EnemyDefeatedEvent(enemyType, experience);
            _eventQueue.QueueEvent(evt);
            
            Debug.Log($"Queued: EnemyDefeatedEvent ({enemyType}). Queue count: {_eventQueue.Count}");
        }
        
        private void DispatchAllEvents()
        {
            Debug.Log($"=== Dispatching {_eventQueue.Count} events ===");
            _eventQueue.DispatchEvents();
            Debug.Log("=== All events dispatched ===");
            Debug.Log($"Total Score: {_totalScore}, Total Experience: {_totalExperience}");
        }
        
        private void ClearAllEvents()
        {
            var count = _eventQueue.Count;
            _eventQueue.Clear();
            Debug.Log($"Cleared {count} events from the queue.");
        }
        
        // Event Handlers
        
        private void OnPlayerSpawned(PlayerSpawnedEvent evt)
        {
            Debug.Log($"[DISPATCHED] Player '{evt.PlayerName}' spawned at position {evt.Position}");
        }
        
        private void OnItemCollected(ItemCollectedEvent evt)
        {
            _totalScore += evt.Points;
            Debug.Log($"[DISPATCHED] Item '{evt.ItemName}' collected! +{evt.Points} points. Total: {_totalScore}");
        }
        
        private void OnEnemyDefeated(EnemyDefeatedEvent evt)
        {
            _totalExperience += evt.ExperienceGained;
            Debug.Log($"[DISPATCHED] Enemy '{evt.EnemyType}' defeated! +{evt.ExperienceGained} XP. Total: {_totalExperience}");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (_eventQueue != null)
            {
                _eventQueue.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
                _eventQueue.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
                _eventQueue.Unsubscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
            }
        }
    }
}
