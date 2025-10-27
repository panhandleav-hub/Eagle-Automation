using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Serilog;
using EagleAutomation.Models;
using EagleAutomation.Infrastructure;

namespace EagleAutomation
{
    /// <summary>
    /// Application entry point and global exception handling
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Global application configuration loaded from appsettings.json
        /// </summary>
        public static AppConfiguration Configuration { get; private set; } = new();

        /// <summary>
        /// Application startup - initialize services and show main window
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize application services
            LoadConfiguration();

            // Initialize structured logging
            LoggingConfiguration.Initialize(Configuration);
            Log.Information("Eagle Automation initializing");

            // Global exception handler for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            // TODO: Check for updates (future feature)
        }

        /// <summary>
        /// Application exit - cleanup resources
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application exiting");
            LoggingConfiguration.Shutdown();
            base.OnExit(e);
        }

        /// <summary>
        /// Load configuration from appsettings.json
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    var config = JsonConvert.DeserializeObject<AppConfiguration>(json);

                    if (config != null)
                    {
                        Configuration = config;
                        Console.WriteLine($"Configuration loaded successfully from {configPath}");
                        Console.WriteLine($"Serial Port: {Configuration.SerialPort.PortName} @ {Configuration.SerialPort.BaudRate} baud");
                    }
                    else
                    {
                        Console.WriteLine("Configuration file was empty or invalid, using defaults");
                    }
                }
                else
                {
                    Console.WriteLine($"Configuration file not found at {configPath}, using defaults");

                    // Create default configuration file for user
                    CreateDefaultConfigurationFile(configPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading configuration: {ex.Message}\n\nUsing default settings.",
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                Configuration = new AppConfiguration(); // Use defaults
            }
        }

        /// <summary>
        /// Create a default appsettings.json file if one doesn't exist
        /// </summary>
        private void CreateDefaultConfigurationFile(string configPath)
        {
            try
            {
                var defaultConfig = new AppConfiguration();
                string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Log.Information("Created default configuration file at {ConfigPath}", configPath);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not create default configuration file at {ConfigPath}", configPath);
            }
        }

        /// <summary>
        /// Save current configuration to appsettings.json
        /// </summary>
        public static void SaveConfiguration()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                string json = JsonConvert.SerializeObject(Configuration, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Log.Information("Configuration saved successfully to {ConfigPath}", configPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving configuration to {ConfigPath}", configPath);
                MessageBox.Show(
                    $"Error saving configuration: {ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Handle unhandled exceptions from non-UI threads
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Log.Fatal(exception, "Fatal unhandled exception in application");

            MessageBox.Show(
                $"A fatal error occurred: {exception?.Message}\n\nThe application will now close.",
                "Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        /// <summary>
        /// Handle unhandled exceptions from UI thread
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Unhandled UI exception");

            MessageBox.Show(
                $"An error occurred: {e.Exception.Message}\n\nPlease save your work and restart the application.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            e.Handled = true;
        }
    }
}
