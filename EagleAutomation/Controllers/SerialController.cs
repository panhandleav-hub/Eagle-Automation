using System;
using System.IO.Ports;
using System.Threading;
using Serilog;
using EagleAutomation.Models;

namespace EagleAutomation.Controllers
{
    /// <summary>
    /// Serial Controller - Manages RS-232 communication with Otari Status 18R console
    /// Handles port initialization, command transmission, and response handling
    /// </summary>
    public class SerialController : IDisposable
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SerialController>();
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
        /// Connect to the console using configuration settings
        /// </summary>
        /// <param name="config">Serial port configuration</param>
        /// <returns>True if connection successful</returns>
        public bool Connect(SerialPortConfig config)
        {
            return Connect(config.PortName, config.BaudRate, config.DataBits,
                          config.GetParity(), config.GetStopBits(),
                          config.ReadTimeout, config.WriteTimeout);
        }

        /// <summary>
        /// Connect to the console on specified COM port with custom parameters
        /// </summary>
        /// <param name="portName">COM port (e.g., "COM1")</param>
        /// <param name="baudRate">Baud rate (typically 19200 for Status 18R)</param>
        /// <param name="dataBits">Data bits (default 8)</param>
        /// <param name="parity">Parity setting (default None)</param>
        /// <param name="stopBits">Stop bits (default One)</param>
        /// <param name="readTimeout">Read timeout in milliseconds (default 500)</param>
        /// <param name="writeTimeout">Write timeout in milliseconds (default 500)</param>
        /// <returns>True if connection successful</returns>
        public bool Connect(string portName, int baudRate = 19200, int dataBits = 8,
                           Parity parity = Parity.None, StopBits stopBits = StopBits.One,
                           int readTimeout = 500, int writeTimeout = 500)
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
                        DataBits = dataBits,
                        Parity = parity,
                        StopBits = stopBits,
                        Handshake = Handshake.None,
                        ReadTimeout = readTimeout,
                        WriteTimeout = writeTimeout
                    };

                    // Attach data received handler
                    _serialPort.DataReceived += OnDataReceived;

                    // Open the port
                    _serialPort.Open();

                    // Send initialization command to console
                    // TODO: Replace with actual protocol initialization once reverse engineered
                    // For now, just verify the port opened successfully
                    _isConnected = _serialPort.IsOpen;

                    Log.Information("Serial port connected: {PortName} @ {BaudRate} baud, {DataBits}-{Parity}-{StopBits}",
                        portName, baudRate, dataBits, parity, stopBits);

                    // Notify connection status changed
                    ConnectionStatusChanged?.Invoke(this, _isConnected);

                    return _isConnected;
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);

                    Log.Error(ex, "Serial connection error on {PortName}", portName);

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
                        Log.Information("Serial port disconnected: {PortName}", _serialPort.PortName);
                    }

                    _isConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Error during serial port disconnect");
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
                Log.Debug("Setting automation mode: {Mode}", mode);
                return SendCommand(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error setting automation mode: {Mode}", mode);
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
                Log.Debug("Setting fader level: CH{Channel} = {Level}", channel, level);
                return SendCommand(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error setting fader level: CH{Channel}", channel);
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
                Log.Debug("Setting switch: CH{Channel} {SwitchType} = {State}", channel, switchType, state ? "ON" : "OFF");
                return SendCommand(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error setting switch state: CH{Channel} {SwitchType}", channel, switchType);
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

                    // Log sent command for protocol debugging
                    Log.Debug("TX: {Command}", BitConverter.ToString(command));

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error sending command: {Command}", BitConverter.ToString(command));
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

                // Log received data for protocol debugging
                Log.Debug("RX: {Data} ({ByteCount} bytes)", BitConverter.ToString(buffer), bytesToRead);

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
                Log.Error(ex, "Error receiving data from serial port");
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
