using UnityEngine;
using UnityIoC.Logging;

namespace UnityIoC.Examples
{
    /// <summary>
    /// Example demonstrating logger usage with dependency injection.
    /// Shows how to inject ILogger&lt;T&gt; for automatic name detection.
    /// </summary>
    public class LoggerUsageExample : MonoBehaviour
    {
        private IContainer _container;
        private IExampleService _exampleService;
        
        private void Start()
        {
            Debug.Log("=== Logger Usage Example ===");
            
            // Create and configure container
            _container = new Container();
            
            // Register logging services
            _container.RegisterLogging();
            
            // Register typed logger for ExampleService
            _container.RegisterLogger<ExampleService>();
            
            // Register our example service
            _container.Register<IExampleService, ExampleService>(ServiceLifetime.Singleton);
            
            // Resolve and use the service (it will have a logger injected)
            _exampleService = _container.Resolve<IExampleService>();
            _exampleService.DoWork();
            
            // Example of using ILoggerFactory directly
            DemonstrateLoggerFactory();
        }
        
        private void DemonstrateLoggerFactory()
        {
            Debug.Log("\n=== Logger Factory Example ===");
            
            // Resolve the logger factory
            var loggerFactory = _container.Resolve<ILoggerFactory>();
            
            // Create a logger with a custom name
            var customLogger = loggerFactory.CreateLogger("CustomComponent");
            customLogger.Information("Logger with custom name");
            
            // Create a logger for a specific type
            var typedLogger = loggerFactory.CreateLogger(typeof(LoggerUsageExample));
            typedLogger.Trace("Logger created for specific type");
            
            // Create a logger using generic method
            var genericLogger = loggerFactory.CreateLogger<LoggerUsageExample>();
            genericLogger.Error("Logger created using generic method");
        }
    }
    
    /// <summary>
    /// Example service interface.
    /// </summary>
    public interface IExampleService
    {
        void DoWork();
    }
    
    /// <summary>
    /// Example service that uses dependency-injected logger.
    /// The logger name will automatically be "UnityIoC.Examples.ExampleService".
    /// </summary>
    public class ExampleService : IExampleService
    {
        private readonly ILogger<ExampleService> _logger;
        
        // Logger is automatically injected with the service's fully qualified name
        public ExampleService(ILogger<ExampleService> logger)
        {
            _logger = logger;
            _logger.Information($"ExampleService created with logger named: {_logger.Name}");
        }
        
        public void DoWork()
        {
            _logger.Trace("Starting work...");
            
            // Simulate some work
            _logger.Information("Doing important work");
            
            // Simulate an error condition
            _logger.Error("An error occurred during work");
            
            _logger.Trace("Work completed");
        }
    }
}
