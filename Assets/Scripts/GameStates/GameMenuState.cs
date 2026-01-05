using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Game menu state - handles main menu UI and navigation.
    /// </summary>
    public class GameMenuState : IGameState
    {
        private readonly IGameStateManager _stateManager;
        
        public GameMenuState(IGameStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        public void Enter()
        {
            Debug.Log("Entering Game Menu State");
            // TODO: Load and display menu UI
            // TODO: Enable menu input handlers
        }
        
        public void Update()
        {
            // TODO: Handle menu input and interactions
            
            // Example: Transition to game on key press
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Starting game...");
                _stateManager.TransitionTo<GamePlayState>();
            }
            
            // Example: Transition to editor on key press
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Opening editor...");
                _stateManager.TransitionTo<GameEditorState>();
            }
        }
        
        public void Exit()
        {
            Debug.Log("Exiting Game Menu State");
            // TODO: Hide menu UI
            // TODO: Cleanup menu resources
        }
    }
}
