using System;
using UnityEngine;

namespace UnityIoC.GameStates
{
    /// <summary>
    /// Manages game state transitions and lifecycle.
    /// </summary>
    public class GameStateManager : IGameStateManager
    {
        private readonly IContainer _container;
        private IGameState _currentState;
        
        public IGameState CurrentState => _currentState;
        
        public GameStateManager(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }
        
        /// <summary>
        /// Transitions to a new game state.
        /// </summary>
        public void TransitionTo<TState>() where TState : IGameState
        {
            // Exit current state
            _currentState?.Exit();
            
            // Resolve new state from container
            _currentState = _container.Resolve<TState>();
            
            // Enter new state
            _currentState?.Enter();
            
            Debug.Log($"Transitioned to state: {typeof(TState).Name}");
        }
        
        /// <summary>
        /// Transitions to a new game state with a parameter.
        /// </summary>
        /// <typeparam name="TState">The state type that implements IGameState&lt;TParameter&gt;.</typeparam>
        /// <typeparam name="TParameter">The type of parameter to pass to the state.</typeparam>
        /// <param name="parameter">The parameter to pass to the state's Enter method.</param>
        public void TransitionTo<TState, TParameter>(TParameter parameter) where TState : IGameState<TParameter>
        {
            // Exit current state
            _currentState?.Exit();
            
            // Resolve new state from container
            var newState = _container.Resolve<TState>();
            
            if (newState == null)
            {
                Debug.LogError($"Failed to resolve state: {typeof(TState).Name}");
                return;
            }
            
            _currentState = newState;
            
            // Enter new state with parameter
            newState.Enter(parameter);
            
            Debug.Log($"Transitioned to state: {typeof(TState).Name} with parameter: {parameter}");
        }
        
        /// <summary>
        /// Updates the current state.
        /// </summary>
        public void Update()
        {
            _currentState?.Update();
        }
    }
}
