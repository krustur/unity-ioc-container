using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Interface for game states.
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
}
