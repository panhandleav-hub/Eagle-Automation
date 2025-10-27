using System.IO.Ports;

namespace EagleAutomation.Models
{
    /// <summary>
    /// Root configuration object loaded from appsettings.json
    /// </summary>
    public class AppConfiguration
    {
        public SerialPortConfig SerialPort { get; set; } = new();
        public TimeCodeConfig TimeCode { get; set; } = new();
        public FilePathsConfig FilePaths { get; set; } = new();
        public AutoSaveConfig AutoSave { get; set; } = new();
        public ConsoleConfig Console { get; set; } = new();
        public LoggingConfig Logging { get; set; } = new();
    }

    /// <summary>
    /// Serial port communication settings
    /// </summary>
    public class SerialPortConfig
    {
        public string PortName { get; set; } = "COM1";
        public int BaudRate { get; set; } = 19200;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "One";
        public int ReadTimeout { get; set; } = 500;
        public int WriteTimeout { get; set; } = 500;

        /// <summary>
        /// Convert string parity to enum
        /// </summary>
        public Parity GetParity()
        {
            return Parity switch
            {
                "None" => System.IO.Ports.Parity.None,
                "Odd" => System.IO.Ports.Parity.Odd,
                "Even" => System.IO.Ports.Parity.Even,
                "Mark" => System.IO.Ports.Parity.Mark,
                "Space" => System.IO.Ports.Parity.Space,
                _ => System.IO.Ports.Parity.None
            };
        }

        /// <summary>
        /// Convert string stop bits to enum
        /// </summary>
        public StopBits GetStopBits()
        {
            return StopBits switch
            {
                "None" => System.IO.Ports.StopBits.None,
                "One" => System.IO.Ports.StopBits.One,
                "Two" => System.IO.Ports.StopBits.Two,
                "OnePointFive" => System.IO.Ports.StopBits.OnePointFive,
                _ => System.IO.Ports.StopBits.One
            };
        }
    }

    /// <summary>
    /// Time code synchronization settings
    /// </summary>
    public class TimeCodeConfig
    {
        public string FrameRate { get; set; } = "29.97 DF";
        public string Source { get; set; } = "LTC";
        public bool GeneratorEnabled { get; set; } = false;
        public string StartTime { get; set; } = "00:00:00:00";
        public string EndTime { get; set; } = "01:00:00:00";
        public bool LoopEnabled { get; set; } = false;
    }

    /// <summary>
    /// File system paths for mix files, presets, and exports
    /// </summary>
    public class FilePathsConfig
    {
        public string MixFilesDirectory { get; set; } = "C:\\Eagle\\Mixes";
        public string PresetsDirectory { get; set; } = "C:\\Eagle\\Presets";
        public string ExportDirectory { get; set; } = "C:\\Eagle\\Export";
        public string BackupDirectory { get; set; } = "C:\\Eagle\\Backups";
        public string DiskmixDirectory { get; set; } = "C:\\Data-dm";
    }

    /// <summary>
    /// Auto-save functionality settings
    /// </summary>
    public class AutoSaveConfig
    {
        public bool Enabled { get; set; } = true;
        public int IntervalMinutes { get; set; } = 5;
        public int MaxFiles { get; set; } = 10;
        public bool BackupOnExit { get; set; } = true;
    }

    /// <summary>
    /// Console hardware specifications
    /// </summary>
    public class ConsoleConfig
    {
        public string Model { get; set; } = "Otari Status 18R";
        public int ChannelCount { get; set; } = 32;
        public int VCAGroups { get; set; } = 8;
    }

    /// <summary>
    /// Logging configuration
    /// </summary>
    public class LoggingConfig
    {
        public bool EnableDebugLogging { get; set; } = false;
        public string LogDirectory { get; set; } = "C:\\Eagle\\Logs";
    }
}
