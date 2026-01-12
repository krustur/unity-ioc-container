using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Interface for game states without parameters.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Called when entering this state.
        /// </summary>
        void Enter();
        
        /// <summary>
        /// Called every frame while in this state.
        /// </summary>
        void Update();
        
        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        void Exit();
    }
    
    /// <summary>
    /// Interface for game states that require parameters on entry.
    /// This does NOT inherit from IGameState to enforce parameter usage.
    /// </summary>
    /// <typeparam name="T">The type of parameter required by this state.</typeparam>
    public interface IGameState<T>
    {
        /// <summary>
        /// Called when entering this state with a parameter.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the state.</param>
        void Enter(T parameter);
        
        /// <summary>
        /// Called every frame while in this state.
        /// </summary>
        void Update();
        
        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        void Exit();
    }
}
