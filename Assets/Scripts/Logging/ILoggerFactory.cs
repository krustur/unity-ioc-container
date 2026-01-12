using System;

namespace UnityIoC.Logging
{
    /// <summary>
    /// Factory for creating ILogger instances with automatic name detection.
    /// Used for dependency injection to provide named loggers.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates a logger with the specified name.
        /// </summary>
        /// <param name="name">The name for the logger.</param>
        /// <returns>A logger instance with the specified name.</returns>
        ILogger CreateLogger(string name);
        
        /// <summary>
        /// Creates a logger with the name derived from the specified type.
        /// </summary>
        /// <param name="type">The type to use for the logger name.</param>
        /// <returns>A logger instance named after the type.</returns>
        ILogger CreateLogger(Type type);
        
        /// <summary>
        /// Creates a logger with the name derived from the generic type parameter.
        /// </summary>
        /// <typeparam name="T">The type to use for the logger name.</typeparam>
        /// <returns>A logger instance named after the type.</returns>
        ILogger CreateLogger<T>();
    }
}
