using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Game play state that accepts a level parameter - demonstrates parameterized state transitions.
    /// </summary>
    public class GamePlayStateWithLevel : IGameState<int>
    {
        private readonly IGameStateManager _stateManager;
        private int _currentLevel;
        
        public GamePlayStateWithLevel(IGameStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        /// <summary>
        /// Called when entering this state without parameters (from IGameState).
        /// Uses default level 1.
        /// </summary>
        public void Enter()
        {
            Enter(1);
        }
        
        /// <summary>
        /// Called when entering this state with a level parameter.
        /// </summary>
        /// <param name="levelNumber">The level number to load.</param>
        public void Enter(int levelNumber)
        {
            _currentLevel = levelNumber;
            Debug.Log($"Entering Game Play State - Loading Level {_currentLevel}");
            // TODO: Initialize game systems
            // TODO: Spawn player
            // TODO: Load specific level based on levelNumber
            // TODO: Start game timer
        }
        
        public void Update()
        {
            // TODO: Update game logic
            // TODO: Process player input
            // TODO: Update AI
            // TODO: Check win/lose conditions
            
            // Example: Return to menu on Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Returning to menu...");
                _stateManager.TransitionTo<GameMenuState>();
            }
            
            // Example: Load next level on N key
            if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log($"Loading next level...");
                _stateManager.TransitionTo<GamePlayStateWithLevel, int>(_currentLevel + 1);
            }
        }
        
        public void Exit()
        {
            Debug.Log($"Exiting Game Play State - Level {_currentLevel}");
            // TODO: Save game state
            // TODO: Cleanup game objects
            // TODO: Stop game timer
        }
    }
}
