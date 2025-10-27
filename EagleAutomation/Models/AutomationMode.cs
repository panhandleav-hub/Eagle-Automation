namespace EagleAutomation.Models
{
    /// <summary>
    /// Automation modes supported by the Otari Status 18R console
    /// Based on the Status 18R manual Section 5 - Eagle Automation
    /// </summary>
    public enum AutomationMode
    {
        /// <summary>
        /// READ mode - Plays back previously recorded automation data
        /// Faders and switches follow recorded moves
        /// </summary>
        Read = 0,

        /// <summary>
        /// WRITE mode - Records new automation data
        /// Overwrites any existing automation for the selected channels
        /// </summary>
        Write = 1,

        /// <summary>
        /// UPDATE mode - Updates existing automation
        /// Allows modification of previously recorded automation
        /// </summary>
        Update = 2,

        /// <summary>
        /// TOUCH mode - Records automation only when fader/control is touched
        /// Returns to reading existing automation when released
        /// Useful for punch-in style edits
        /// </summary>
        Touch = 3,

        /// <summary>
        /// ISOLATE mode - Isolates selected channels from automation playback
        /// Allows manual control while other channels play back automation
        /// </summary>
        Isolate = 4,

        /// <summary>
        /// GLIDE mode - Smoothly transitions from current position to automated position
        /// Prevents sudden jumps when engaging automation
        /// </summary>
        Glide = 5
    }
}
