using System;
using System.IO;
using System.Text;

namespace UnityIoC.Logging
{
    /// <summary>
    /// High-performance logger implementation with support for Unity console and file logging.
    /// Optimized for dependency injection with automatic name detection.
    /// </summary>
    public class Logger : ILogger
    {
        private readonly string _name;
        private readonly bool _isUnityEnvironment;
        private readonly string _logFilePath;
        private readonly object _fileLock = new object();
        
        // Pre-allocated StringBuilder for performance
        private readonly StringBuilder _messageBuilder;
        
        // Cached log level prefixes for performance
        private const string TracePrefix = "[TRACE]";
        private const string InfoPrefix = "[INFO]";
        private const string ErrorPrefix = "[ERROR]";
        
        /// <summary>
        /// Gets the name of the logger (fully qualified class name).
        /// </summary>
        public string Name => _name;
        
        /// <summary>
        /// Creates a new Logger instance with the specified name.
        /// </summary>
        /// <param name="name">The name of the logger (typically the fully qualified class name).</param>
        public Logger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            _name = name;
            
            // Performance optimization: Pre-allocate StringBuilder with reasonable capacity
            _messageBuilder = new StringBuilder(256);
            
            // Detect if running in Unity environment
            _isUnityEnvironment = DetectUnityEnvironment();
            
            // Setup file logging for non-Unity environments
            if (!_isUnityEnvironment)
            {
                // Use application directory for logs
                var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                
                // Ensure log directory exists
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                
                // Generate log file name with date for daily log rotation
                // Note: Each logger instance created on a different day will use a new file,
                // which is intentional to provide daily log rotation
                var logFileName = $"app_{DateTime.Now:yyyy-MM-dd}.log";
                _logFilePath = Path.Combine(logDirectory, logFileName);
            }
        }
        
        /// <summary>
        /// Logs a trace-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Trace(string message)
        {
            LogMessage(TracePrefix, message);
        }
        
        /// <summary>
        /// Logs an information-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Information(string message)
        {
            LogMessage(InfoPrefix, message);
        }
        
        /// <summary>
        /// Logs an error-level message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message)
        {
            LogMessage(ErrorPrefix, message);
        }
        
        /// <summary>
        /// Core logging method that formats and outputs messages.
        /// Uses lock for thread-safety. For high-concurrency scenarios,
        /// consider using ThreadStatic StringBuilder or object pooling.
        /// </summary>
        private void LogMessage(string levelPrefix, string message)
        {
            // Performance: Build message once and reuse
            string formattedMessage;
            
            lock (_messageBuilder)
            {
                _messageBuilder.Clear();
                _messageBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                _messageBuilder.Append(" ");
                _messageBuilder.Append(levelPrefix);
                _messageBuilder.Append(" [");
                _messageBuilder.Append(_name);
                _messageBuilder.Append("] ");
                _messageBuilder.Append(message);
                
                formattedMessage = _messageBuilder.ToString();
            }
            
            if (_isUnityEnvironment)
            {
                LogToUnity(levelPrefix, formattedMessage);
            }
            else
            {
                LogToFile(formattedMessage);
            }
        }
        
        /// <summary>
        /// Logs a message to Unity's console.
        /// </summary>
        private void LogToUnity(string levelPrefix, string message)
        {
            // Use reflection to call UnityEngine.Debug methods to avoid hard dependency
            var debugType = Type.GetType("UnityEngine.Debug, UnityEngine");
            if (debugType == null)
            {
                // Fallback to console if Unity is not available
                Console.WriteLine(message);
                return;
            }
            
            // Select appropriate Unity Debug method based on level
            if (levelPrefix == ErrorPrefix)
            {
                var errorMethod = debugType.GetMethod("LogError", new[] { typeof(object) });
                errorMethod?.Invoke(null, new object[] { message });
            }
            else
            {
                var logMethod = debugType.GetMethod("Log", new[] { typeof(object) });
                logMethod?.Invoke(null, new object[] { message });
            }
        }
        
        /// <summary>
        /// Logs a message to a file with proper locking for thread safety.
        /// Uses File.AppendAllText for simplicity and reliability.
        /// Note: Opens/closes file for each write. For high-frequency logging,
        /// consider using a buffered StreamWriter approach instead.
        /// </summary>
        private void LogToFile(string message)
        {
            try
            {
                // Thread-safe file writing with lock
                lock (_fileLock)
                {
                    // Append to file efficiently
                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"Failed to write log to file: {ex.Message}");
                Console.WriteLine(message);
            }
        }
        
        /// <summary>
        /// Detects if the application is running in Unity environment.
        /// </summary>
        private static bool DetectUnityEnvironment()
        {
            // Check if UnityEngine assembly is available
            var unityEngineType = Type.GetType("UnityEngine.Application, UnityEngine");
            return unityEngineType != null;
        }
    }
}
