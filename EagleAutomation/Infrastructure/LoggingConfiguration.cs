using System;
using System.IO;
using Serilog;
using Serilog.Events;
using EagleAutomation.Models;

namespace EagleAutomation.Infrastructure
{
    /// <summary>
    /// Configures Serilog for structured logging throughout the application
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Initialize Serilog with file and debug output sinks
        /// </summary>
        public static void Initialize(AppConfiguration config)
        {
            // Determine log level from configuration
            var logLevel = config.Logging.EnableDebugLogging
                ? LogEventLevel.Debug
                : LogEventLevel.Information;

            // Ensure log directory exists
            string logDirectory = config.Logging.LogDirectory;
            if (!Directory.Exists(logDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logDirectory);
                }
                catch (Exception ex)
                {
                    // Fall back to application directory if we can't create the configured path
                    logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    Directory.CreateDirectory(logDirectory);
                    Console.WriteLine($"Could not create configured log directory, using: {logDirectory}. Error: {ex.Message}");
                }
            }

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "EagleAutomation")
                .Enrich.WithProperty("Version", "1.0.0")
                .WriteTo.Debug(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "eagle-automation-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    fileSizeLimitBytes: 10_485_760, // 10 MB
                    rollOnFileSizeLimit: true)
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "protocol-capture-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff}] {Message:lj}{NewLine}",
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    fileSizeLimitBytes: 52_428_800) // 50 MB for protocol captures
                .CreateLogger();

            Log.Information("=== Eagle Automation Started ===");
            Log.Information("Log Level: {LogLevel}", logLevel);
            Log.Information("Log Directory: {LogDirectory}", logDirectory);
        }

        /// <summary>
        /// Close and flush the logger on application exit
        /// </summary>
        public static void Shutdown()
        {
            Log.Information("=== Eagle Automation Shutting Down ===");
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Create a context logger for a specific class
        /// </summary>
        public static ILogger ForContext<T>()
        {
            return Log.ForContext<T>();
        }

        /// <summary>
        /// Create a context logger for a specific context name
        /// </summary>
        public static ILogger ForContext(string contextName)
        {
            return Log.ForContext("SourceContext", contextName);
        }
    }
}
