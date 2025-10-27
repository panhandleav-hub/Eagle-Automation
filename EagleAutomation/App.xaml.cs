using System;
using System.Windows;

namespace EagleAutomation
{
    /// <summary>
    /// Application entry point and global exception handling
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application startup - initialize services and show main window
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Global exception handler for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            // TODO: Initialize application services here
            // - Load configuration from appsettings.json
            // - Initialize logging
            // - Check for updates (future feature)
        }

        /// <summary>
        /// Handle unhandled exceptions from non-UI threads
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
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
