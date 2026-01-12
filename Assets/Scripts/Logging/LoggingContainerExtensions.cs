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
    }
}
