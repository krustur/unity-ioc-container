using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Game play state - handles active gameplay.
    /// </summary>
    public class GamePlayState : IGameState
    {
        private readonly IGameStateManager _stateManager;
        
        public GamePlayState(IGameStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        public void Enter()
        {
            Debug.Log("Entering Game Play State");
            // TODO: Initialize game systems
            // TODO: Spawn player
            // TODO: Load level
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
        }
        
        public void Exit()
        {
            Debug.Log("Exiting Game Play State");
            // TODO: Save game state
            // TODO: Cleanup game objects
            // TODO: Stop game timer
        }
    }
}
