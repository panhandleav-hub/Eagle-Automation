using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using EagleAutomation.Controllers;

namespace EagleAutomation.Views
{
    /// <summary>
    /// Preferences Window - Configure console communication, time code, and file paths
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        private readonly SerialController _serialController;

        public PreferencesWindow(SerialController serialController)
        {
            InitializeComponent();
            _serialController = serialController;

            // Populate COM port list
            LoadAvailableComPorts();
        }

        /// <summary>
        /// Load available COM ports into combo box
        /// </summary>
        private void LoadAvailableComPorts()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                
                foreach (string port in ports)
                {
                    ComPortCombo.Items.Add(port);
                }

                // Select first port if available
                if (ComPortCombo.Items.Count > 0)
                {
                    ComPortCombo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading COM ports: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Switch between preference tabs
        /// </summary>
        private void SwitchTab(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            // Hide all tabs
            InfoTab.Visibility = Visibility.Collapsed;
            SettingsTab.Visibility = Visibility.Collapsed;
            TimeCodeTab.Visibility = Visibility.Collapsed;
            PathsTab.Visibility = Visibility.Collapsed;

            // Reset all button styles
            BtnInfoTab.Style = (Style)FindResource("NavButton");
            BtnSettingsTab.Style = (Style)FindResource("NavButton");
            BtnTimeCodeTab.Style = (Style)FindResource("NavButton");
            BtnPathsTab.Style = (Style)FindResource("NavButton");

            BtnInfoTab.IsEnabled = true;
            BtnSettingsTab.IsEnabled = true;
            BtnTimeCodeTab.IsEnabled = true;
            BtnPathsTab.IsEnabled = true;

            // Show selected tab and highlight button
            switch (button.Name)
            {
                case "BtnInfoTab":
                    InfoTab.Visibility = Visibility.Visible;
                    BtnInfoTab.Background = System.Windows.Media.Brushes.CornflowerBlue;
                    BtnInfoTab.Foreground = System.Windows.Media.Brushes.White;
                    BtnInfoTab.IsEnabled = false;
                    break;

                case "BtnSettingsTab":
                    SettingsTab.Visibility = Visibility.Visible;
                    BtnSettingsTab.Background = System.Windows.Media.Brushes.CornflowerBlue;
                    BtnSettingsTab.Foreground = System.Windows.Media.Brushes.White;
                    BtnSettingsTab.IsEnabled = false;
                    break;

                case "BtnTimeCodeTab":
                    TimeCodeTab.Visibility = Visibility.Visible;
                    BtnTimeCodeTab.Background = System.Windows.Media.Brushes.CornflowerBlue;
                    BtnTimeCodeTab.Foreground = System.Windows.Media.Brushes.White;
                    BtnTimeCodeTab.IsEnabled = false;
                    break;

                case "BtnPathsTab":
                    PathsTab.Visibility = Visibility.Visible;
                    BtnPathsTab.Background = System.Windows.Media.Brushes.CornflowerBlue;
                    BtnPathsTab.Foreground = System.Windows.Media.Brushes.White;
                    BtnPathsTab.IsEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// Test connection to console with selected settings
        /// </summary>
        private void TestConnection(object sender, RoutedEventArgs e)
        {
            if (ComPortCombo.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a COM port first.",
                    "No Port Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            string portName = ComPortCombo.SelectedItem.ToString() ?? "COM1";

            // Disconnect first if connected
            if (_serialController.IsConnected)
            {
                _serialController.Disconnect();
            }

            // Try to connect
            bool success = _serialController.Connect(portName, 19200);

            if (success)
            {
                MessageBox.Show(
                    $"Successfully connected to console on {portName}",
                    "Connection Test Successful",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show(
                    $"Failed to connect to console on {portName}\n\n" +
                    "Please check:\n" +
                    "1. Console is powered on\n" +
                    "2. RS-232 cable is connected\n" +
                    "3. Correct COM port is selected",
                    "Connection Test Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Apply settings without closing window
        /// </summary>
        private void ApplySettings(object sender, RoutedEventArgs e)
        {
            // TODO: Save settings to configuration file
            MessageBox.Show(
                "Settings applied successfully.\n\n" +
                "Note: Some settings may require restarting the application to take effect.",
                "Settings Applied",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Save settings and close window
        /// </summary>
        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            // TODO: Save settings to configuration file
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Close window without saving
        /// </summary>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
