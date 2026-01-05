using UnityEngine;

namespace UnityIoC
{
    /// <summary>
    /// Base class for game configuration ScriptableObjects.
    /// Extend this class to create custom configuration objects that can be
    /// registered as singletons in the IoC container.
    /// </summary>
    public abstract class GameConfiguration : ScriptableObject
    {
        /// <summary>
        /// Called when the configuration is registered in the container.
        /// Override this to perform any initialization logic.
        /// </summary>
        public virtual void OnRegistered()
        {
            Debug.Log($"Configuration '{name}' registered in IoC container.");
        }
    }
}
