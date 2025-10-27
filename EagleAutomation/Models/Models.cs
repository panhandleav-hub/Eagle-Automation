namespace EagleAutomation.Models
{
    /// <summary>
    /// Types of switches available on the Status 18R console
    /// </summary>
    public enum SwitchType
    {
        /// <summary>
        /// Mute switch - Silences channel output
        /// </summary>
        Mute = 0,

        /// <summary>
        /// Solo switch - Solos channel (mutes all others)
        /// </summary>
        Solo = 1,

        /// <summary>
        /// EQ switch - Enables/disables EQ on channel
        /// </summary>
        EQ = 2,

        /// <summary>
        /// Insert switch - Enables/disables insert effects
        /// </summary>
        Insert = 3,

        /// <summary>
        /// Dynamics switch - Enables/disables dynamics processing
        /// </summary>
        Dynamics = 4
    }

    /// <summary>
    /// Message received from console
    /// </summary>
    public class ConsoleMessage
    {
        /// <summary>
        /// Type of message received
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Channel number (if applicable)
        /// </summary>
        public int? Channel { get; set; }

        /// <summary>
        /// Raw data bytes
        /// </summary>
        public byte[] Data { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Parsed value (if applicable)
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Error message (if error type)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Types of messages that can be received from console
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Command acknowledgment
        /// </summary>
        Acknowledgment,

        /// <summary>
        /// Fader position update
        /// </summary>
        FaderUpdate,

        /// <summary>
        /// Switch state update
        /// </summary>
        SwitchUpdate,

        /// <summary>
        /// Time code sync message
        /// </summary>
        TimeCodeSync,

        /// <summary>
        /// Error message
        /// </summary>
        Error,

        /// <summary>
        /// Unknown message type
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents a single channel strip on the console
    /// </summary>
    public class ChannelStrip
    {
        /// <summary>
        /// Channel number (1-32)
        /// </summary>
        public int ChannelNumber { get; set; }

        /// <summary>
        /// Channel label/name
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Current fader position (0-1023 or protocol-specific range)
        /// </summary>
        public int FaderLevel { get; set; }

        /// <summary>
        /// Mute switch state
        /// </summary>
        public bool IsMuted { get; set; }

        /// <summary>
        /// Solo switch state
        /// </summary>
        public bool IsSoloed { get; set; }

        /// <summary>
        /// EQ enabled state
        /// </summary>
        public bool IsEQEnabled { get; set; }

        /// <summary>
        /// Current automation mode for this channel
        /// </summary>
        public AutomationMode AutomationMode { get; set; }

        /// <summary>
        /// Is this channel selected for editing
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// VCA group assignment (0 = none, 1-8 = group number)
        /// </summary>
        public int VCAGroup { get; set; }
    }

    /// <summary>
    /// Mix file data structure
    /// </summary>
    public class MixFile
    {
        /// <summary>
        /// Mix file name
        /// </summary>
        public string Name { get; set; } = "Untitled";

        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Last modified timestamp
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// SMPTE time code start
        /// </summary>
        public string TimeCodeStart { get; set; } = "00:00:00:00";

        /// <summary>
        /// SMPTE time code end
        /// </summary>
        public string TimeCodeEnd { get; set; } = "00:00:00:00";

        /// <summary>
        /// Frame rate (24, 25, 29.97 DF/NDF, 30)
        /// </summary>
        public string FrameRate { get; set; } = "29.97 DF";

        /// <summary>
        /// Channel strip data
        /// </summary>
        public List<ChannelStrip> Channels { get; set; } = new List<ChannelStrip>();

        /// <summary>
        /// Automation events
        /// </summary>
        public List<AutomationEvent> Events { get; set; } = new List<AutomationEvent>();
    }

    /// <summary>
    /// Automation event (marker, cue, etc.)
    /// </summary>
    public class AutomationEvent
    {
        /// <summary>
        /// Event time code
        /// </summary>
        public string TimeCode { get; set; } = "00:00:00:00";

        /// <summary>
        /// Event name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Event type
        /// </summary>
        public string EventType { get; set; } = "Marker";

        /// <summary>
        /// Event description/notes
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
