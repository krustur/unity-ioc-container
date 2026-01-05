using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Game editor state - handles level/content editing functionality.
    /// </summary>
    public class GameEditorState : IGameState
    {
        private readonly IGameStateManager _stateManager;
        
        public GameEditorState(IGameStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        public void Enter()
        {
            Debug.Log("Entering Game Editor State");
            // TODO: Initialize editor tools
            // TODO: Load editor UI
            // TODO: Setup editor camera
        }
        
        public void Update()
        {
            // TODO: Handle editor input and interactions
            // TODO: Process editor tools (placement, deletion, etc.)
            
            // Example: Return to menu on Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Returning to menu...");
                _stateManager.TransitionTo<GameMenuState>();
            }
            
            // Example: Test play from editor
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Testing gameplay...");
                _stateManager.TransitionTo<GamePlayState>();
            }
        }
        
        public void Exit()
        {
            Debug.Log("Exiting Game Editor State");
            // TODO: Save editor state
            // TODO: Cleanup editor UI
            // TODO: Reset camera
        }
    }
}
