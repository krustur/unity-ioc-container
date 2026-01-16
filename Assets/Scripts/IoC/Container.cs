using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIoC
{
    /// <summary>
    /// High-performance IoC container optimized for Unity 6.0.
    /// Uses dictionary-based lookups and minimizes allocations.
    /// </summary>
    public class Container : IContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services;
        private readonly Dictionary<Type, object> _singletonInstances;
        
        public Container()
        {
            _services = new Dictionary<Type, ServiceDescriptor>(64); // Pre-allocate for performance
            _singletonInstances = new Dictionary<Type, object>(32);
        }
        
        /// <summary>
        /// Registers a service as itself.
        /// </summary>
        public void Register<TService>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
        {
            Register<TService, TService>(lifetime);
        }
        
        /// <summary>
        /// Registers a service with a concrete implementation type.
        /// </summary>
        public void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TImplementation : TService
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);
            
            var descriptor = new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = implementationType,
                Lifetime = lifetime,
                Factory = null
            };
            
            _services[serviceType] = descriptor;
        }
        
        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        public void Register<TService>(Func<IContainer, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            
            var serviceType = typeof(TService);
            
            var descriptor = new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = null,
                Lifetime = lifetime,
                Factory = container => factory(container)
            };
            
            _services[serviceType] = descriptor;
        }
        
        /// <summary>
        /// Registers a singleton instance directly.
        /// </summary>
        public void RegisterInstance<TService>(TService instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            
            var serviceType = typeof(TService);
            
            var descriptor = new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = instance.GetType(),
                Lifetime = ServiceLifetime.Singleton,
                Factory = null
            };
            
            _services[serviceType] = descriptor;
            _singletonInstances[serviceType] = instance;
        }
        
        /// <summary>
        /// Resolves a service from the container.
        /// </summary>
        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }
        
        /// <summary>
        /// Resolves a service from the container by type.
        /// </summary>
        public object Resolve(Type serviceType)
        {
            if (!_services.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
            }
            
            // Check if singleton instance already exists
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                if (_singletonInstances.TryGetValue(serviceType, out var singletonInstance))
                {
                    return singletonInstance;
                }
            }
            
            // Create instance
            object instance;
            
            if (descriptor.Factory != null)
            {
                instance = descriptor.Factory(this);
            }
            else
            {
                instance = CreateInstance(descriptor.ImplementationType);
            }
            
            // Cache singleton instances
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                _singletonInstances[serviceType] = instance;
            }
            
            return instance;
        }
        
        /// <summary>
        /// Checks if a service is registered.
        /// </summary>
        public bool IsRegistered<TService>()
        {
            return IsRegistered(typeof(TService));
        }
        
        /// <summary>
        /// Checks if a service is registered by type.
        /// </summary>
        public bool IsRegistered(Type serviceType)
        {
            return _services.ContainsKey(serviceType);
        }
        
        /// <summary>
        /// Gets all registered service types in the container.
        /// </summary>
        /// <returns>An enumerable of all registered service types.</returns>
        public IEnumerable<Type> GetRegisteredTypes()
        {
            return _services.Keys;
        }
        
        /// <summary>
        /// Gets the implementation type for a registered service type.
        /// Returns null if the service is registered with a factory or if not registered.
        /// </summary>
        /// <param name="serviceType">The service type to query.</param>
        /// <returns>The implementation type, or null if not available.</returns>
        public Type GetImplementationType(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var descriptor))
            {
                return descriptor.ImplementationType;
            }
            return null;
        }
        
        /// <summary>
        /// Creates an instance of the specified type with constructor injection.
        /// </summary>
        private object CreateInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors();
            
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"No public constructor found for type {implementationType.Name}.");
            }
            
            // Try to find the best constructor by checking which ones have all dependencies registered
            System.Reflection.ConstructorInfo bestConstructor = null;
            int maxResolvableParams = -1;
            
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                
                // Parameterless constructor is always the best choice
                if (parameters.Length == 0)
                {
                    return Activator.CreateInstance(implementationType);
                }
                
                // Check if all parameters can be resolved
                bool allResolvable = true;
                int resolvableCount = 0;
                
                foreach (var param in parameters)
                {
                    if (_services.ContainsKey(param.ParameterType))
                    {
                        resolvableCount++;
                    }
                    else
                    {
                        allResolvable = false;
                        break;
                    }
                }
                
                // Select constructor with most resolvable dependencies
                if (allResolvable && resolvableCount > maxResolvableParams)
                {
                    bestConstructor = constructor;
                    maxResolvableParams = resolvableCount;
                }
            }
            
            if (bestConstructor == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find a suitable constructor for type {implementationType.Name}. " +
                    "Ensure all constructor dependencies are registered in the container.");
            }
            
            // Resolve constructor parameters
            var constructorParams = bestConstructor.GetParameters();
            var parameterInstances = new object[constructorParams.Length];
            
            for (int i = 0; i < constructorParams.Length; i++)
            {
                parameterInstances[i] = Resolve(constructorParams[i].ParameterType);
            }
            
            return Activator.CreateInstance(implementationType, parameterInstances);
        }
        
        /// <summary>
        /// Internal descriptor for service registration.
        /// </summary>
        private class ServiceDescriptor
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
            public ServiceLifetime Lifetime { get; set; }
            public Func<IContainer, object> Factory { get; set; }
        }
    }
}
