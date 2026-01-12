using System;
using System.Collections.Generic;

namespace UnityIoC.Logging
{
    /// <summary>
    /// High-performance logger factory with caching support.
    /// Creates and caches logger instances to minimize allocations.
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        // Cache loggers by name for performance
        private readonly Dictionary<string, ILogger> _loggerCache;
        private readonly object _cacheLock = new object();
        
        /// <summary>
        /// Initializes a new instance of LoggerFactory.
        /// </summary>
        public LoggerFactory()
        {
            // Pre-allocate cache for performance
            _loggerCache = new Dictionary<string, ILogger>(32);
        }
        
        /// <summary>
        /// Creates a logger with the specified name.
        /// Returns a cached instance if available.
        /// </summary>
        /// <param name="name">The name for the logger.</param>
        /// <returns>A logger instance with the specified name.</returns>
        public ILogger CreateLogger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            // Thread-safe cache lookup and creation
            lock (_cacheLock)
            {
                if (_loggerCache.TryGetValue(name, out var cachedLogger))
                {
                    return cachedLogger;
                }
                
                var logger = new Logger(name);
                _loggerCache[name] = logger;
                return logger;
            }
        }
        
        /// <summary>
        /// Creates a logger with the name derived from the specified type.
        /// The name will be the fully qualified type name (namespace + class name).
        /// </summary>
        /// <param name="type">The type to use for the logger name.</param>
        /// <returns>A logger instance named after the type.</returns>
        public ILogger CreateLogger(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            
            // Use FullName for fully qualified namespace + class name
            var name = type.FullName ?? type.Name;
            return CreateLogger(name);
        }
        
        /// <summary>
        /// Creates a logger with the name derived from the generic type parameter.
        /// The name will be the fully qualified type name (namespace + class name).
        /// </summary>
        /// <typeparam name="T">The type to use for the logger name.</typeparam>
        /// <returns>A logger instance named after the type.</returns>
        public ILogger CreateLogger<T>()
        {
            return CreateLogger(typeof(T));
        }
    }
}
