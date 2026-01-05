using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Manages transitions between game states.
    /// </summary>
    public interface IGameStateManager
    {
        /// <summary>
        /// Gets the current active game state.
        /// </summary>
        IGameState CurrentState { get; }
        
        /// <summary>
        /// Transitions to a new game state.
        /// </summary>
        void TransitionTo<TState>() where TState : IGameState;
        
        /// <summary>
        /// Updates the current state.
        /// </summary>
        void Update();
    }
}
