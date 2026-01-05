namespace UnityIoC
{
    /// <summary>
    /// Defines the lifetime of a service in the IoC container.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A new instance is created every time the service is requested.
        /// </summary>
        Transient,
        
        /// <summary>
        /// A single instance is created and shared across all requests.
        /// </summary>
        Singleton
    }
}
