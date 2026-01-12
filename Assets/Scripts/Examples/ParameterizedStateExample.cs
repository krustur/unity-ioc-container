using UnityEngine;
using UnityIoC;
using UnityIoC.GameStates;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example of a parameterized game state that accepts level data.
    /// </summary>
    public class LevelState : IGameState<int>
    {
        private readonly IGameStateManager _stateManager;
        private int _levelNumber;
        
        public LevelState(IGameStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        // Parameterless Enter (from IGameState) - provides a default
        public void Enter()
        {
            Enter(1); // Default to level 1
        }
        
        // Parameterized Enter (from IGameState<int>)
        public void Enter(int levelNumber)
        {
            _levelNumber = levelNumber;
            Debug.Log($"Entering Level {_levelNumber}");
            // Initialize level-specific content
        }
        
        public void Update()
        {
            // Level logic here
            
            // Example: Load next level on completion
            if (Input.GetKeyDown(KeyCode.N))
            {
                _stateManager.TransitionTo<LevelState, int>(_levelNumber + 1);
            }
        }
        
        public void Exit()
        {
            Debug.Log($"Exiting Level {_levelNumber}");
        }
    }
    
    /// <summary>
    /// Example of a parameterized game state that accepts complex data.
    /// </summary>
    public class BattleState : IGameState<BattleConfiguration>
    {
        private readonly IGameStateManager _stateManager;
        private BattleConfiguration _config;
        
        public BattleState(IGameStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        // Parameterless Enter - provides a default configuration
        public void Enter()
        {
            Enter(new BattleConfiguration { EnemyCount = 3, Difficulty = "Normal" });
        }
        
        // Parameterized Enter with configuration
        public void Enter(BattleConfiguration config)
        {
            _config = config;
            Debug.Log($"Starting battle: {_config.EnemyCount} enemies, Difficulty: {_config.Difficulty}");
            // Setup battle based on configuration
        }
        
        public void Update()
        {
            // Battle logic here
        }
        
        public void Exit()
        {
            Debug.Log("Battle ended");
        }
    }
    
    /// <summary>
    /// Configuration data class for BattleState.
    /// </summary>
    public class BattleConfiguration
    {
        public int EnemyCount { get; set; }
        public string Difficulty { get; set; }
        public string BattleType { get; set; }
    }
    
    /// <summary>
    /// Example showing how to use parameterized state transitions.
    /// </summary>
    public class ParameterizedStateExample : MonoBehaviour
    {
        private IContainer _container;
        private IGameStateManager _stateManager;
        
        private void Awake()
        {
            // Setup container
            _container = new Container();
            _container.RegisterInstance<IContainer>(_container);
            _container.Register<IGameStateManager, GameStateManager>(ServiceLifetime.Singleton);
            
            // Register parameterized states
            _container.Register<LevelState, LevelState>(ServiceLifetime.Transient);
            _container.Register<BattleState, BattleState>(ServiceLifetime.Transient);
            
            // Get state manager
            _stateManager = _container.Resolve<IGameStateManager>();
            
            // Example transitions:
            DemonstrateTransitions();
        }
        
        private void DemonstrateTransitions()
        {
            Debug.Log("=== Parameterized State Transition Examples ===");
            
            // Example 1: Transition to level with specific number
            Debug.Log("\n1. Transition to Level 5:");
            _stateManager.TransitionTo<LevelState, int>(5);
            
            // Example 2: Transition to level without parameter (uses default)
            Debug.Log("\n2. Transition to Level (default):");
            _stateManager.TransitionTo<LevelState>();
            
            // Example 3: Transition to battle with configuration
            Debug.Log("\n3. Transition to Battle with configuration:");
            var battleConfig = new BattleConfiguration
            {
                EnemyCount = 10,
                Difficulty = "Hard",
                BattleType = "Boss"
            };
            _stateManager.TransitionTo<BattleState, BattleConfiguration>(battleConfig);
            
            Debug.Log("\n=== Examples Complete ===");
        }
        
        private void Update()
        {
            _stateManager?.Update();
            
            // Example hotkeys for testing
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _stateManager.TransitionTo<LevelState, int>(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _stateManager.TransitionTo<LevelState, int>(2);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                var config = new BattleConfiguration
                {
                    EnemyCount = 5,
                    Difficulty = "Normal"
                };
                _stateManager.TransitionTo<BattleState, BattleConfiguration>(config);
            }
        }
    }
}
