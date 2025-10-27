using System;
using System.IO.Ports;
using System.Threading;
using EagleAutomation.Models;

namespace EagleAutomation.Controllers
{
    /// <summary>
    /// Serial Controller - Manages RS-232 communication with Otari Status 18R console
    /// Handles port initialization, command transmission, and response handling
    /// </summary>
    public class SerialController : IDisposable
    {
        private SerialPort? _serialPort;
        private readonly object _lock = new object();
        private bool _isConnected;

        /// <summary>
        /// Current connection status
        /// </summary>
        public bool IsConnected => _isConnected && _serialPort?.IsOpen == true;

        /// <summary>
        /// Event fired when connection status changes
        /// </summary>
        public event EventHandler<bool>? ConnectionStatusChanged;

        /// <summary>
        /// Event fired when data is received from console
        /// </summary>
        public event EventHandler<byte[]>? DataReceived;

        /// <summary>
        /// Connect to the console on specified COM port
        /// </summary>
        /// <param name="portName">COM port (e.g., "COM1")</param>
        /// <param name="baudRate">Baud rate (typically 19200 for Status 18R)</param>
        /// <returns>True if connection successful</returns>
        public bool Connect(string portName, int baudRate = 19200)
        {
            lock (_lock)
            {
                try
                {
                    // Close existing connection if any
                    Disconnect();

                    // Configure serial port based on Status 18R specifications
                    // These settings match the manual's requirements for RS-232 communication
                    _serialPort = new SerialPort
                    {
                        PortName = portName,
                        BaudRate = baudRate,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,
                        Handshake = Handshake.None,
                        ReadTimeout = 500,
                        WriteTimeout = 500
                    };

                    // Attach data received handler
                    _serialPort.DataReceived += OnDataReceived;

                    // Open the port
                    _serialPort.Open();

                    // Send initialization command to console
                    // TODO: Replace with actual protocol initialization once reverse engineered
                    // For now, just verify the port opened successfully
                    _isConnected = _serialPort.IsOpen;

                    // Notify connection status changed
                    ConnectionStatusChanged?.Invoke(this, _isConnected);

                    return _isConnected;
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);
                    
                    // Log error (TODO: Add proper logging framework)
                    System.Diagnostics.Debug.WriteLine($"Serial connection error: {ex.Message}");
                    
                    return false;
                }
            }
        }

        /// <summary>
        /// Disconnect from the console
        /// </summary>
        public void Disconnect()
        {
            lock (_lock)
            {
                try
                {
                    if (_serialPort?.IsOpen == true)
                    {
                        // Send shutdown command to console if needed
                        // TODO: Add graceful shutdown command once protocol is known

                        _serialPort.Close();
                    }

                    _isConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disconnecting: {ex.Message}");
                }
                finally
                {
                    _serialPort?.Dispose();
                    _serialPort = null;
                }
            }
        }

        /// <summary>
        /// Set automation mode on console
        /// </summary>
        /// <param name="mode">Desired automation mode</param>
        /// <returns>True if command sent successfully</returns>
        public bool SetAutomationMode(AutomationMode mode)
        {
            if (!IsConnected)
                return false;

            try
            {
                // TODO: Replace with actual protocol command once reverse engineered
                // This is a placeholder for the actual command structure
                // 
                // Expected protocol format (to be determined from Portmon captures):
                // - Command byte(s) for mode selection
                // - Possible channel mask
                // - Checksum or terminator
                //
                // Example placeholder:
                // byte[] command = mode switch
                // {
                //     AutomationMode.Read => new byte[] { 0x01, 0x10 },
                //     AutomationMode.Write => new byte[] { 0x01, 0x20 },
                //     AutomationMode.Update => new byte[] { 0x01, 0x30 },
                //     AutomationMode.Touch => new byte[] { 0x01, 0x40 },
                //     AutomationMode.Isolate => new byte[] { 0x01, 0x50 },
                //     AutomationMode.Glide => new byte[] { 0x01, 0x60 },
                //     _ => new byte[] { 0x01, 0x10 }
                // };

                byte[] command = ProtocolHandler.BuildAutomationModeCommand(mode);
                return SendCommand(command);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting automation mode: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Set fader level for a specific channel
        /// </summary>
        /// <param name="channel">Channel number (1-32)</param>
        /// <param name="level">Fader level (0-1023 or similar, TBD from protocol)</param>
        /// <returns>True if command sent successfully</returns>
        public bool SetFaderLevel(int channel, int level)
        {
            if (!IsConnected)
                return false;

            if (channel < 1 || channel > 32)
                throw new ArgumentOutOfRangeException(nameof(channel), "Channel must be between 1 and 32");

            try
            {
                // TODO: Build actual fader command from reverse engineered protocol
                byte[] command = ProtocolHandler.BuildFaderCommand(channel, level);
                return SendCommand(command);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting fader level: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Set switch state (mute, solo, etc.) for a channel
        /// </summary>
        /// <param name="channel">Channel number (1-32)</param>
        /// <param name="switchType">Type of switch (Mute, Solo, etc.)</param>
        /// <param name="state">On or Off</param>
        /// <returns>True if command sent successfully</returns>
        public bool SetSwitchState(int channel, SwitchType switchType, bool state)
        {
            if (!IsConnected)
                return false;

            try
            {
                // TODO: Build actual switch command from reverse engineered protocol
                byte[] command = ProtocolHandler.BuildSwitchCommand(channel, switchType, state);
                return SendCommand(command);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting switch state: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send raw command to console
        /// </summary>
        /// <param name="command">Command bytes to send</param>
        /// <returns>True if sent successfully</returns>
        private bool SendCommand(byte[] command)
        {
            lock (_lock)
            {
                try
                {
                    if (_serialPort?.IsOpen != true)
                        return false;

                    _serialPort.Write(command, 0, command.Length);
                    
                    // Log sent command for debugging
                    System.Diagnostics.Debug.WriteLine($"Sent command: {BitConverter.ToString(command)}");
                    
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending command: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Handle incoming data from console
        /// </summary>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort?.IsOpen != true)
                    return;

                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead == 0)
                    return;

                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);

                // Log received data for debugging
                System.Diagnostics.Debug.WriteLine($"Received data: {BitConverter.ToString(buffer)}");

                // TODO: Parse received data according to protocol
                // Different message types might include:
                // - Acknowledgments
                // - Current fader positions
                // - Switch states
                // - Error codes

                // Notify listeners of received data
                DataReceived?.Invoke(this, buffer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error receiving data: {ex.Message}");
            }
        }

        /// <summary>
        /// Get list of available COM ports on the system
        /// </summary>
        /// <returns>Array of COM port names</returns>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Dispose of serial port resources
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
