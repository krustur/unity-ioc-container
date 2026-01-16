namespace UnityIoC.Logging
{
    /// <summary>
    /// Generic logger interface that automatically derives the logger name from the type parameter.
    /// This allows for easier dependency injection where the logger name matches the consuming class.
    /// </summary>
    /// <typeparam name="T">The type that identifies this logger (typically the consuming class).</typeparam>
    public interface ILogger<T> : ILogger
    {
    }
}
