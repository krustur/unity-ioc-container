namespace UnityIoC.Logging
{
    /// <summary>
    /// Generic logger implementation that wraps a named logger.
    /// The logger name is automatically derived from the type parameter.
    /// </summary>
    /// <typeparam name="T">The type that identifies this logger.</typeparam>
    public class Logger<T> : ILogger<T>
    {
        private readonly ILogger _innerLogger;
        
        /// <summary>
        /// Creates a new generic logger instance using the provided logger factory.
        /// </summary>
        /// <param name="loggerFactory">The factory to create the underlying logger.</param>
        public Logger(ILoggerFactory loggerFactory)
        {
            _innerLogger = loggerFactory.CreateLogger<T>();
        }
        
        /// <summary>
        /// Gets the name of the logger (fully qualified class name of T).
        /// </summary>
        public string Name => _innerLogger.Name;
        
        /// <summary>
        /// Logs a trace-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Trace(string message)
        {
            _innerLogger.Trace(message);
        }
        
        /// <summary>
        /// Logs an information-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Information(string message)
        {
            _innerLogger.Information(message);
        }
        
        /// <summary>
        /// Logs an error-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message)
        {
            _innerLogger.Error(message);
        }
    }
}
