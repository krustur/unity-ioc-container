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
        /// Transitions to a new game state with a parameter.
        /// </summary>
        /// <typeparam name="TState">The state type that implements IGameState&lt;TParameter&gt;.</typeparam>
        /// <typeparam name="TParameter">The type of parameter to pass to the state.</typeparam>
        /// <param name="parameter">The parameter to pass to the state's Enter method.</param>
        void TransitionTo<TState, TParameter>(TParameter parameter) where TState : IGameState<TParameter>;
        
        /// <summary>
        /// Updates the current state.
        /// </summary>
        void Update();
    }
}
