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
            
            // Example: Transition to editor on key press
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Opening editor...");
                _stateManager.TransitionTo<GameEditorState>();
            }
            
            // Example: Transition to game with specific level (parameterized state)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Starting game at level 1...");
                _stateManager.TransitionTo<GamePlayState, int>(1);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Starting game at level 2...");
                _stateManager.TransitionTo<GamePlayState, int>(2);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Starting game at level 3...");
                _stateManager.TransitionTo<GamePlayState, int>(3);
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
