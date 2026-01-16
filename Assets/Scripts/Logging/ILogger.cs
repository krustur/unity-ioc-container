namespace UnityIoC.Logging
{
    /// <summary>
    /// Interface for logging with support for multiple log levels.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the name of the logger (typically the fully qualified class name).
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Logs a trace-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Trace(string message);
        
        /// <summary>
        /// Logs an information-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Information(string message);
        
        /// <summary>
        /// Logs an error-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message);
    }
}
