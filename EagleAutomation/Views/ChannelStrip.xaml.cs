using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Serilog;
using EagleAutomation.Controllers;
using EagleAutomation.Models;

namespace EagleAutomation.Views
{
    /// <summary>
    /// Interactive Channel Strip Control - Fader, Mute, Solo, EQ, Automation
    /// </summary>
    public partial class ChannelStrip : UserControl
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<ChannelStrip>();
        private SerialController? _serialController;
        private int _channelNumber;
        private bool _isUpdatingFromCode = false;

        /// <summary>
        /// Channel number (1-32)
        /// </summary>
        public int ChannelNumber
        {
            get => _channelNumber;
            set
            {
                _channelNumber = value;
                ChannelLabel.Text = $"CH {_channelNumber}";
            }
        }

        /// <summary>
        /// Serial controller for sending commands to console
        /// </summary>
        public SerialController? SerialController
        {
            get => _serialController;
            set => _serialController = value;
        }

        /// <summary>
        /// Current fader level (0-100)
        /// </summary>
        public double FaderLevel
        {
            get => FaderSlider.Value;
            set
            {
                _isUpdatingFromCode = true;
                FaderSlider.Value = value;
                LevelDisplay.Text = ((int)value).ToString();
                _isUpdatingFromCode = false;
            }
        }

        /// <summary>
        /// Mute state
        /// </summary>
        public bool IsMuted
        {
            get => MuteButton.IsChecked == true;
            set
            {
                MuteButton.IsChecked = value;
                UpdateMuteButtonAppearance();
            }
        }

        /// <summary>
        /// Solo state
        /// </summary>
        public bool IsSoloed
        {
            get => SoloButton.IsChecked == true;
            set
            {
                SoloButton.IsChecked = value;
                UpdateSoloButtonAppearance();
            }
        }

        /// <summary>
        /// EQ enabled state
        /// </summary>
        public bool IsEqEnabled
        {
            get => EqButton.IsChecked == true;
            set
            {
                EqButton.IsChecked = value;
                UpdateEqButtonAppearance();
            }
        }

        /// <summary>
        /// Automation enabled state
        /// </summary>
        public bool IsAutomationEnabled
        {
            get => AutoButton.IsChecked == true;
            set
            {
                AutoButton.IsChecked = value;
                UpdateAutomationIndicator();
            }
        }

        /// <summary>
        /// Current automation mode for this channel
        /// </summary>
        private AutomationMode _automationMode = AutomationMode.Read;

        public ChannelStrip()
        {
            InitializeComponent();
            ChannelNumber = 1;
        }

        /// <summary>
        /// Handle fader value changes
        /// </summary>
        private void OnFaderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdatingFromCode)
                return;

            // Update level display
            int level = (int)e.NewValue;
            LevelDisplay.Text = level.ToString();

            // Send command to console
            if (_serialController != null && _serialController.IsConnected)
            {
                // Convert 0-100 scale to whatever the protocol expects (e.g., 0-1023)
                // For now, we'll send the 0-100 value directly
                // TODO: Adjust scaling once protocol is known
                bool success = _serialController.SetFaderLevel(_channelNumber, level);

                if (!success)
                {
                    Log.Warning("Failed to send fader command for CH{Channel}", _channelNumber);
                }
            }
        }

        /// <summary>
        /// Handle mute button click
        /// </summary>
        private void OnMuteClicked(object sender, RoutedEventArgs e)
        {
            bool isMuted = MuteButton.IsChecked == true;
            UpdateMuteButtonAppearance();

            // Send command to console
            if (_serialController != null && _serialController.IsConnected)
            {
                bool success = _serialController.SetSwitchState(
                    _channelNumber,
                    SwitchType.Mute,
                    isMuted);

                if (!success)
                {
                    Log.Warning("Failed to send mute command for CH{Channel}", _channelNumber);
                }
            }
        }

        /// <summary>
        /// Handle solo button click
        /// </summary>
        private void OnSoloClicked(object sender, RoutedEventArgs e)
        {
            bool isSoloed = SoloButton.IsChecked == true;
            UpdateSoloButtonAppearance();

            // Send command to console
            if (_serialController != null && _serialController.IsConnected)
            {
                bool success = _serialController.SetSwitchState(
                    _channelNumber,
                    SwitchType.Solo,
                    isSoloed);

                if (!success)
                {
                    Log.Warning("Failed to send solo command for CH{Channel}", _channelNumber);
                }
            }
        }

        /// <summary>
        /// Handle EQ button click
        /// </summary>
        private void OnEqClicked(object sender, RoutedEventArgs e)
        {
            bool isEnabled = EqButton.IsChecked == true;
            UpdateEqButtonAppearance();

            // Send command to console
            if (_serialController != null && _serialController.IsConnected)
            {
                bool success = _serialController.SetSwitchState(
                    _channelNumber,
                    SwitchType.EQ,
                    isEnabled);

                if (!success)
                {
                    Log.Warning("Failed to send EQ command for CH{Channel}", _channelNumber);
                }
            }
        }

        /// <summary>
        /// Handle auto button click
        /// </summary>
        private void OnAutoClicked(object sender, RoutedEventArgs e)
        {
            UpdateAutomationIndicator();
            // Automation enable/disable logic would go here
        }

        /// <summary>
        /// Update mute button appearance based on state
        /// </summary>
        private void UpdateMuteButtonAppearance()
        {
            if (MuteButton.IsChecked == true)
            {
                MuteButton.Background = new SolidColorBrush(Color.FromRgb(255, 51, 102)); // Red
                MuteButton.Foreground = Brushes.White;
            }
            else
            {
                MuteButton.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                MuteButton.Foreground = Brushes.White;
            }
        }

        /// <summary>
        /// Update solo button appearance based on state
        /// </summary>
        private void UpdateSoloButtonAppearance()
        {
            if (SoloButton.IsChecked == true)
            {
                SoloButton.Background = new SolidColorBrush(Color.FromRgb(255, 204, 0)); // Yellow
                SoloButton.Foreground = Brushes.Black;
            }
            else
            {
                SoloButton.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                SoloButton.Foreground = Brushes.White;
            }
        }

        /// <summary>
        /// Update EQ button appearance based on state
        /// </summary>
        private void UpdateEqButtonAppearance()
        {
            if (EqButton.IsChecked == true)
            {
                EqButton.Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)); // Blue
                EqButton.Foreground = Brushes.White;
            }
            else
            {
                EqButton.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                EqButton.Foreground = Brushes.White;
            }
        }

        /// <summary>
        /// Update automation indicator based on auto button state
        /// </summary>
        private void UpdateAutomationIndicator()
        {
            if (AutoButton.IsChecked == true)
            {
                // Show current automation mode
                AutomationLed.Fill = _automationMode switch
                {
                    AutomationMode.Read => new SolidColorBrush(Color.FromRgb(0, 255, 136)), // Green
                    AutomationMode.Write => new SolidColorBrush(Color.FromRgb(255, 51, 102)), // Red
                    AutomationMode.Update => new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange
                    AutomationMode.Touch => new SolidColorBrush(Color.FromRgb(0, 255, 255)), // Cyan
                    AutomationMode.Isolate => new SolidColorBrush(Color.FromRgb(128, 128, 128)), // Gray
                    AutomationMode.Glide => new SolidColorBrush(Color.FromRgb(255, 105, 180)), // Pink
                    _ => new SolidColorBrush(Color.FromRgb(136, 136, 136))
                };

                AutomationText.Text = _automationMode.ToString().ToUpper();
                AutomationText.Foreground = AutomationLed.Fill;
            }
            else
            {
                // Off state
                AutomationLed.Fill = new SolidColorBrush(Color.FromRgb(136, 136, 136));
                AutomationText.Text = "OFF";
                AutomationText.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
            }
        }

        /// <summary>
        /// Set the automation mode for this channel
        /// </summary>
        public void SetAutomationMode(AutomationMode mode)
        {
            _automationMode = mode;
            UpdateAutomationIndicator();
        }
    }
}
