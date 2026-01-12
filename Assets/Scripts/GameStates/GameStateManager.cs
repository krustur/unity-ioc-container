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
        private object _currentState; // Changed to object to support both IGameState and IGameState<T>
        
        public IGameState CurrentState => _currentState as IGameState;
        
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
            ExitCurrentState();
            
            // Resolve new state from container
            _currentState = _container.Resolve<TState>();
            
            // Enter new state
            if (_currentState is IGameState state)
            {
                state.Enter();
            }
            
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
            ExitCurrentState();
            
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
            if (_currentState is IGameState state)
            {
                state.Update();
            }
            else if (_currentState != null)
            {
                // For IGameState<T> states, we need to call Update via reflection or dynamic
                var updateMethod = _currentState.GetType().GetMethod("Update");
                updateMethod?.Invoke(_currentState, null);
            }
        }
        
        /// <summary>
        /// Exits the current state regardless of its type.
        /// </summary>
        private void ExitCurrentState()
        {
            if (_currentState is IGameState state)
            {
                state.Exit();
            }
            else if (_currentState != null)
            {
                // For IGameState<T> states, we need to call Exit via reflection
                var exitMethod = _currentState.GetType().GetMethod("Exit");
                exitMethod?.Invoke(_currentState, null);
            }
        }
    }
}
