using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityIoC.Logging
{
    /// <summary>
    /// Extension methods for registering logging services with the IoC container.
    /// </summary>
    public static class LoggingContainerExtensions
    {
        /// <summary>
        /// Registers logging services (ILoggerFactory) with the container as a singleton.
        /// After calling this method, services can inject ILoggerFactory to create loggers,
        /// or use RegisterLogger&lt;T&gt; to register specific typed loggers.
        /// </summary>
        /// <param name="container">The IoC container to register services with.</param>
        public static void RegisterLogging(this IContainer container)
        {
            // Register the logger factory as a singleton
            container.Register<ILoggerFactory, LoggerFactory>(ServiceLifetime.Singleton);
        }
        
        /// <summary>
        /// Registers a typed logger ILogger&lt;T&gt; for a specific type T.
        /// This allows a service of type T to inject ILogger&lt;T&gt; and get a logger
        /// automatically named with T's fully qualified class name.
        /// </summary>
        /// <typeparam name="T">The type to create a logger for.</typeparam>
        /// <param name="container">The IoC container to register the logger with.</param>
        public static void RegisterLogger<T>(this IContainer container)
        {
            container.Register<ILogger<T>, Logger<T>>(ServiceLifetime.Singleton);
        }
        
        /// <summary>
        /// Automatically registers all required ILogger&lt;T&gt; dependencies by analyzing
        /// constructor parameters of all registered services in the container.
        /// This should be called after all service registrations are complete.
        /// </summary>
        /// <param name="container">The IoC container to analyze and register loggers for.</param>
        public static void RegisterAllLoggers(this IContainer container)
        {
            // Get all registered types
            var registeredTypes = container.GetRegisteredTypes().ToList();
            
            // Track logger types we need to register (use HashSet to avoid duplicates)
            var loggerTypesToRegister = new HashSet<Type>();
            
            foreach (var serviceType in registeredTypes)
            {
                // Get the implementation type for this service
                var implementationType = container.GetImplementationType(serviceType);
                
                // Skip if we don't have an implementation type (factory registrations)
                if (implementationType == null)
                    continue;
                
                // Get all constructors
                var constructors = implementationType.GetConstructors();
                
                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    
                    foreach (var parameter in parameters)
                    {
                        var paramType = parameter.ParameterType;
                        
                        // Check if this is an ILogger<T> type
                        if (paramType.IsGenericType && 
                            paramType.GetGenericTypeDefinition() == typeof(ILogger<>))
                        {
                            // Add to the set of logger types to register
                            loggerTypesToRegister.Add(paramType);
                        }
                    }
                }
            }
            
            // Register all discovered logger types
            foreach (var loggerType in loggerTypesToRegister)
            {
                // Check if already registered
                if (container.IsRegistered(loggerType))
                    continue;
                
                // Get the generic argument (the T in ILogger<T>)
                var genericArg = loggerType.GetGenericArguments()[0];
                
                // Create the Logger<T> type
                var loggerImplType = typeof(Logger<>).MakeGenericType(genericArg);
                
                // Register using reflection since we can't use generic method directly
                // Find: void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
                var registerMethod = typeof(IContainer).GetMethods()
                    .FirstOrDefault(m => m.Name == "Register" && 
                           m.IsGenericMethodDefinition && 
                           m.GetGenericArguments().Length == 2 &&
                           m.GetParameters().Length == 1 &&
                           m.GetParameters()[0].ParameterType == typeof(ServiceLifetime));
                
                if (registerMethod == null)
                    throw new InvalidOperationException("Unable to find Register<TService, TImplementation>(ServiceLifetime) method on IContainer.");
                
                var genericRegisterMethod = registerMethod.MakeGenericMethod(loggerType, loggerImplType);
                genericRegisterMethod.Invoke(container, new object[] { ServiceLifetime.Singleton });
            }
        }
    }
}
