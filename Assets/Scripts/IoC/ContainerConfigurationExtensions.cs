using System;
using System.Reflection;
using UnityEngine;

namespace UnityIoC
{
    /// <summary>
    /// Extension methods for IContainer to support ScriptableObject configuration registration.
    /// </summary>
    public static class ContainerConfigurationExtensions
    {
        // Cache the MethodInfo for RegisterInstance to avoid repeated reflection
        private static MethodInfo _registerInstanceMethod;
        
        /// <summary>
        /// Registers a GameConfiguration ScriptableObject by its concrete type.
        /// Uses reflection internally but caches the MethodInfo for better performance.
        /// </summary>
        /// <param name="container">The container to register the configuration in</param>
        /// <param name="configuration">The configuration instance to register</param>
        public static void RegisterConfiguration(this IContainer container, GameConfiguration configuration)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
                
            if (configuration == null)
            {
                Debug.LogWarning("Attempted to register null configuration, skipping.");
                return;
            }
            
            // Get or cache the RegisterInstance method
            if (_registerInstanceMethod == null)
            {
                _registerInstanceMethod = typeof(IContainer).GetMethod(nameof(IContainer.RegisterInstance));
                if (_registerInstanceMethod == null)
                {
                    throw new InvalidOperationException("Could not find RegisterInstance method on IContainer");
                }
            }
            
            // Register the configuration by its concrete type
            var configType = configuration.GetType();
            var genericMethod = _registerInstanceMethod.MakeGenericMethod(configType);
            genericMethod.Invoke(container, new object[] { configuration });
            
            // Call the OnRegistered callback
            configuration.OnRegistered();
        }
        
        /// <summary>
        /// Registers multiple GameConfiguration ScriptableObjects.
        /// </summary>
        /// <param name="container">The container to register configurations in</param>
        /// <param name="configurations">Array of configurations to register</param>
        public static void RegisterConfigurations(this IContainer container, GameConfiguration[] configurations)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
                
            if (configurations == null || configurations.Length == 0)
            {
                Debug.Log("No configuration objects to register.");
                return;
            }
            
            Debug.Log($"Registering {configurations.Length} configuration object(s)...");
            
            foreach (var config in configurations)
            {
                if (config == null)
                {
                    Debug.LogWarning("Null configuration found in array, skipping.");
                    continue;
                }
                
                container.RegisterConfiguration(config);
            }
        }
    }
}
