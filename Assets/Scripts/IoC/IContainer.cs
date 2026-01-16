using System;
using System.Collections.Generic;

namespace UnityIoC
{
    /// <summary>
    /// Interface for the IoC container.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Registers a service as itself.
        /// </summary>
        void Register<TService>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class;
        
        /// <summary>
        /// Registers a service with a concrete implementation type.
        /// </summary>
        void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TImplementation : TService;
        
        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        void Register<TService>(Func<IContainer, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Transient);
        
        /// <summary>
        /// Registers a singleton instance.
        /// </summary>
        void RegisterInstance<TService>(TService instance);
        
        /// <summary>
        /// Resolves a service from the container.
        /// </summary>
        TService Resolve<TService>();
        
        /// <summary>
        /// Resolves a service from the container by type.
        /// </summary>
        object Resolve(Type serviceType);
        
        /// <summary>
        /// Checks if a service is registered in the container.
        /// </summary>
        bool IsRegistered<TService>();
        
        /// <summary>
        /// Checks if a service is registered in the container by type.
        /// </summary>
        bool IsRegistered(Type serviceType);
        
        /// <summary>
        /// Gets all registered service types in the container.
        /// </summary>
        /// <returns>An enumerable of all registered service types.</returns>
        IEnumerable<Type> GetRegisteredTypes();
        
        /// <summary>
        /// Gets the implementation type for a registered service type.
        /// Returns null if the service is registered with a factory or if not registered.
        /// </summary>
        /// <param name="serviceType">The service type to query.</param>
        /// <returns>The implementation type, or null if not available.</returns>
        Type GetImplementationType(Type serviceType);
    }
}
