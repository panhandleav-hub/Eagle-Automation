using System;
using System.IO.Ports;
using System.Linq;
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

            // Populate COM port list and load current configuration
            LoadAvailableComPorts();
            LoadCurrentConfiguration();
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

                // Will select configured port in LoadCurrentConfiguration()
                if (ComPortCombo.Items.Count > 0 && ComPortCombo.SelectedItem == null)
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
        /// Load current configuration into UI controls
        /// </summary>
        private void LoadCurrentConfiguration()
        {
            try
            {
                var config = App.Configuration;

                // Serial Port Settings
                if (ComPortCombo.Items.Contains(config.SerialPort.PortName))
                {
                    ComPortCombo.SelectedItem = config.SerialPort.PortName;
                }

                // Baud Rate
                BaudRateCombo.SelectedItem = BaudRateCombo.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == config.SerialPort.BaudRate.ToString());

                // Data Bits
                DataBitsCombo.SelectedItem = DataBitsCombo.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == config.SerialPort.DataBits.ToString());

                // Parity
                ParityCombo.SelectedItem = ParityCombo.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == config.SerialPort.Parity);

                // File Paths
                MixFilesPathText.Text = config.FilePaths.MixFilesDirectory;
                PresetsPathText.Text = config.FilePaths.PresetsDirectory;
                BackupPathText.Text = config.FilePaths.BackupDirectory;
                DiskmixPathText.Text = config.FilePaths.DiskmixDirectory;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading configuration: {ex.Message}",
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
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
            int baudRate = GetSelectedBaudRate();
            int dataBits = GetSelectedDataBits();
            Parity parity = GetSelectedParity();

            // Disconnect first if connected
            if (_serialController.IsConnected)
            {
                _serialController.Disconnect();
            }

            // Try to connect with current UI settings
            bool success = _serialController.Connect(portName, baudRate, dataBits, parity);

            if (success)
            {
                MessageBox.Show(
                    $"Successfully connected to console\n" +
                    $"Port: {portName}\n" +
                    $"Baud Rate: {baudRate}\n" +
                    $"Data Bits: {dataBits}\n" +
                    $"Parity: {parity}",
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
            try
            {
                SaveConfigurationFromUI();

                MessageBox.Show(
                    "Settings applied successfully.\n\n" +
                    "Note: Some settings may require reconnecting to the console to take effect.",
                    "Settings Applied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving settings: {ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Save settings and close window
        /// </summary>
        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveConfigurationFromUI();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving settings: {ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Save configuration from UI controls to App.Configuration and appsettings.json
        /// </summary>
        private void SaveConfigurationFromUI()
        {
            // Update configuration object with values from UI
            if (ComPortCombo.SelectedItem != null)
            {
                App.Configuration.SerialPort.PortName = ComPortCombo.SelectedItem.ToString() ?? "COM1";
            }

            App.Configuration.SerialPort.BaudRate = GetSelectedBaudRate();
            App.Configuration.SerialPort.DataBits = GetSelectedDataBits();
            App.Configuration.SerialPort.Parity = GetSelectedParityString();

            // File Paths
            App.Configuration.FilePaths.MixFilesDirectory = MixFilesPathText.Text;
            App.Configuration.FilePaths.PresetsDirectory = PresetsPathText.Text;
            App.Configuration.FilePaths.BackupDirectory = BackupPathText.Text;
            App.Configuration.FilePaths.DiskmixDirectory = DiskmixPathText.Text;

            // Save to appsettings.json
            App.SaveConfiguration();
        }

        /// <summary>
        /// Get selected baud rate from combo box
        /// </summary>
        private int GetSelectedBaudRate()
        {
            if (BaudRateCombo.SelectedItem is ComboBoxItem item)
            {
                return int.Parse(item.Content.ToString() ?? "19200");
            }
            return 19200;
        }

        /// <summary>
        /// Get selected data bits from combo box
        /// </summary>
        private int GetSelectedDataBits()
        {
            if (DataBitsCombo.SelectedItem is ComboBoxItem item)
            {
                return int.Parse(item.Content.ToString() ?? "8");
            }
            return 8;
        }

        /// <summary>
        /// Get selected parity as enum
        /// </summary>
        private Parity GetSelectedParity()
        {
            if (ParityCombo.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString() switch
                {
                    "None" => Parity.None,
                    "Odd" => Parity.Odd,
                    "Even" => Parity.Even,
                    "Mark" => Parity.Mark,
                    "Space" => Parity.Space,
                    _ => Parity.None
                };
            }
            return Parity.None;
        }

        /// <summary>
        /// Get selected parity as string
        /// </summary>
        private string GetSelectedParityString()
        {
            if (ParityCombo.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString() ?? "None";
            }
            return "None";
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
