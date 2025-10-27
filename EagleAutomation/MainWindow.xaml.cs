using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Serilog;
using EagleAutomation.Views;
using EagleAutomation.Controllers;
using EagleAutomation.Models;

namespace EagleAutomation
{
    /// <summary>
    /// Main Console Window - Primary interface for automation control
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<MainWindow>();
        private readonly SerialController _serialController;
        private readonly DispatcherTimer _timeCodeTimer;
        private TimeSpan _currentTimeCode;
        private readonly List<ChannelStrip> _channelStrips;
        private AutomationMode _currentAutomationMode = AutomationMode.Read;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize serial controller
            _serialController = new SerialController();

            // Initialize channel strips collection
            _channelStrips = new List<ChannelStrip>();

            // Initialize timecode display timer
            _timeCodeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33) // ~30fps update
            };
            _timeCodeTimer.Tick += UpdateTimeCodeDisplay;
            _timeCodeTimer.Start();

            // Initialize connection to console
            InitializeConsoleConnection();

            // Create all 32 channel strips
            CreateChannelStrips();
        }

        /// <summary>
        /// Initialize serial connection to the console
        /// </summary>
        private void InitializeConsoleConnection()
        {
            try
            {
                // Load COM port settings from configuration
                var config = App.Configuration.SerialPort;
                bool connected = _serialController.Connect(config);

                if (connected)
                {
                    MessageBox.Show(
                        $"Successfully connected to {App.Configuration.Console.Model}\n" +
                        $"Port: {config.PortName} @ {config.BaudRate} baud",
                        "Connection Established",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Could not connect to console. Please check:\n" +
                        "1. Console is powered on\n" +
                        "2. RS-232 cable is connected to AUTOMATION PC port\n" +
                        $"3. Correct COM port ({config.PortName}) is selected\n\n" +
                        "You can change settings in Preferences.",
                        "Connection Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing serial connection: {ex.Message}",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Update time code display (simulated for now)
        /// TODO: Replace with actual time code from console or external source
        /// </summary>
        private void UpdateTimeCodeDisplay(object? sender, EventArgs e)
        {
            // TODO: Get actual time code from serial protocol
            // For now, just increment a simulated time code
            _currentTimeCode = _currentTimeCode.Add(TimeSpan.FromMilliseconds(33));
            
            int hours = _currentTimeCode.Hours;
            int minutes = _currentTimeCode.Minutes;
            int seconds = _currentTimeCode.Seconds;
            int frames = (int)(_currentTimeCode.Milliseconds * 30 / 1000); // Simulating 30fps

            TimeCodeDisplay.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}:{frames:D2}";
        }

        /// <summary>
        /// Create all 32 channel strips and add to panel
        /// </summary>
        private void CreateChannelStrips()
        {
            int channelCount = App.Configuration.Console.ChannelCount;

            for (int i = 1; i <= channelCount; i++)
            {
                var channelStrip = new ChannelStrip
                {
                    ChannelNumber = i,
                    SerialController = _serialController,
                    FaderLevel = 75, // Default fader position
                    IsAutomationEnabled = false
                };

                _channelStrips.Add(channelStrip);
                ChannelStripsPanel.Children.Add(channelStrip);
            }

            // Set initial automation mode on all strips
            UpdateAllChannelAutomation(_currentAutomationMode);
        }

        /// <summary>
        /// Handle channels per row selection change
        /// </summary>
        private void OnChannelsPerRowChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelsPerRowCombo.SelectedItem is ComboBoxItem item)
            {
                int channelsPerRow = int.Parse(item.Content.ToString() ?? "8");

                // Update WrapPanel width to accommodate the selected number of channels
                // Each channel strip is ~110 pixels wide (100 + margins)
                ChannelStripsPanel.MaxWidth = channelsPerRow * 110;
            }
        }

        /// <summary>
        /// Update automation mode on all channel strips
        /// </summary>
        private void UpdateAllChannelAutomation(AutomationMode mode)
        {
            foreach (var strip in _channelStrips)
            {
                strip.SetAutomationMode(mode);
            }
        }

        /// <summary>
        /// Set automation mode on the console
        /// </summary>
        private void SetAutomationMode(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.Button button)
                return;

            // Determine which mode was selected
            AutomationMode mode = button.Name switch
            {
                "BtnRead" => AutomationMode.Read,
                "BtnWrite" => AutomationMode.Write,
                "BtnUpdate" => AutomationMode.Update,
                "BtnTouch" => AutomationMode.Touch,
                "BtnIsolate" => AutomationMode.Isolate,
                "BtnGlide" => AutomationMode.Glide,
                _ => AutomationMode.Read
            };

            // Send command to console via serial
            bool success = _serialController.SetAutomationMode(mode);

            if (success)
            {
                // Save current mode
                _currentAutomationMode = mode;

                // Update all channel strips to show the new mode
                UpdateAllChannelAutomation(mode);

                // Update UI to show active mode
                // Reset all buttons to secondary style
                BtnRead.Style = (Style)FindResource("SecondaryButton");
                BtnWrite.Style = (Style)FindResource("SecondaryButton");
                BtnUpdate.Style = (Style)FindResource("SecondaryButton");
                BtnTouch.Style = (Style)FindResource("SecondaryButton");
                BtnIsolate.Style = (Style)FindResource("SecondaryButton");
                BtnGlide.Style = (Style)FindResource("SecondaryButton");

                // Highlight selected button
                button.Style = (Style)FindResource("PrimaryButton");

                Log.Information("Automation mode changed to: {Mode}", mode);
            }
            else
            {
                MessageBox.Show(
                    "Failed to set automation mode. Check console connection.",
                    "Command Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        /// <summary>
        /// Navigate to Mix Edit window
        /// </summary>
        private void ShowMixEdit(object sender, RoutedEventArgs e)
        {
            var mixEditWindow = new MixEditWindow();
            mixEditWindow.Show();
            // Optionally: this.Close(); // if you want to close the main window
        }

        /// <summary>
        /// Navigate to Events window
        /// </summary>
        private void ShowEvents(object sender, RoutedEventArgs e)
        {
            var eventsWindow = new EventsWindow();
            eventsWindow.Show();
        }

        /// <summary>
        /// Navigate to Track Sheet window
        /// </summary>
        private void ShowTrackSheet(object sender, RoutedEventArgs e)
        {
            var trackSheetWindow = new TrackSheetWindow();
            trackSheetWindow.Show();
        }

        /// <summary>
        /// Navigate to Preferences window
        /// </summary>
        private void ShowPreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new PreferencesWindow(_serialController);
            preferencesWindow.ShowDialog(); // Modal dialog
        }

        /// <summary>
        /// Clean up resources when window closes
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            // Stop timecode timer
            _timeCodeTimer?.Stop();

            // Disconnect serial port
            _serialController?.Disconnect();
        }
    }
}
