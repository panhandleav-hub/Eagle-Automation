# Eagle Automation - Otari Status 18R Controller

Modern replacement for the original Eagle Automation software that controlled the Otari Status 18R mixing console on Windows 98SE.

## 🎯 Project Goal

Reverse engineer and recreate the Eagle Automation software to enable continued use of the vintage Otari Status 18R professional mixing console with modern Windows systems. The original software only ran on Windows 98SE and is no longer compatible with current hardware/OS.

## 📋 Current Status

**Phase: Protocol Reverse Engineering**

- ✅ Core application structure complete
- ✅ User interface foundation ready
- ✅ Serial communication framework implemented
- ⏳ **NEXT STEP:** Capture serial protocol data using Portmon on Win95 PC
- ⏳ Decode and implement actual protocol commands
- ⏳ Test and validate against real console hardware

## 🔧 Features

### Implemented
- Modern WPF desktop application
- Dark theme matching professional audio software aesthetics
- Serial port (RS-232) communication framework
- Console window with channel strips (visual mockup)
- Automation mode selection UI (READ, WRITE, UPDATE, TOUCH, ISOLATE, GLIDE)
- Preferences window for COM port configuration
- Time code display framework
- Configuration file management

### To Be Implemented (After Protocol Analysis)
- Actual console protocol commands
- Real-time fader position control
- Switch automation (mute, solo, EQ)
- Mix file save/load functionality
- Automation curve editing
- Time code synchronization
- Events and cue management

## 🚀 Getting Started

### Prerequisites

- **Visual Studio 2022** (Community Edition is free)
- **.NET 8.0 SDK** or later
- **Windows 10/11** (64-bit)
- **RS-232 Serial Port** (built-in or USB-to-serial adapter)
- **Otari Status 18R Console** (for testing with real hardware)

### Installation

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd EagleAutomation
   ```

2. **Open in Visual Studio:**
   - Double-click `EagleAutomation.sln`
   - Visual Studio will restore NuGet packages automatically

3. **Build the solution:**
   - Press `Ctrl+Shift+B` or
   - Menu: Build → Build Solution

4. **Run the application:**
   - Press `F5` (Debug) or `Ctrl+F5` (Release)

### Configuration

Edit `appsettings.json` to configure:
- COM port settings
- Time code preferences
- File paths
- Auto-save options

## 📡 Serial Communication

### Console Connection

The Otari Status 18R connects via RS-232 serial cable:
- **Console Port:** "AUTOMATION PC" on rear panel (9-pin D-sub)
- **PC Port:** COM1, COM2, etc. (or USB-to-serial adapter)
- **Cable:** Standard straight-through RS-232 cable
- **UART:** Requires 16550AF compatible UART chip

### Protocol Specifications (From Manual)

Based on Status 18R manual, the system uses:
- **Communication:** RS-232 serial
- **Connector:** 9-pin D-sub
- **UART:** 16550AF chip required
- **Control:** VCA-based fader automation
- **Modes:** READ, WRITE, UPDATE, TOUCH, ISOLATE, GLIDE

**Protocol details (baud rate, packet structure, commands) are being reverse engineered.**

## 🔬 Reverse Engineering Workflow

### Phase 1: Data Capture (Current)

1. **Setup Win95 PC with original Eagle software**
   - Install Portmon (serial port monitor)
   - Connect to Status 18R console
   - Run original Eagle automation software

2. **Capture Serial Data**
   - Monitor COM port during Eagle operation
   - Record data for each operation:
     - Setting automation modes (READ, WRITE, etc.)
     - Moving faders
     - Pressing MUTE/SOLO buttons
     - All console interactions
   - Save capture logs for analysis

3. **Documentation**
   - Screenshot Portmon captures
   - Document which UI action produced which serial data
   - Note timing and sequences

### Phase 2: Protocol Analysis

1. **Identify Command Structure**
   - Find command headers/footers
   - Determine message length encoding
   - Identify checksums/CRC
   - Map command bytes to functions

2. **Decode Data Encoding**
   - Fader position encoding (resolution, range)
   - Channel number encoding
   - Switch state encoding
   - Time code format

3. **Document Protocol**
   - Create protocol specification document
   - Note all discovered commands
   - Document data formats

### Phase 3: Implementation (Next)

1. **Update `ProtocolHandler.cs`**
   - Replace placeholder methods with real protocol
   - Implement command builders
   - Add response parsing

2. **Test Each Command**
   - Test against real console
   - Verify fader movements
   - Verify switch operations
   - Validate automation modes

3. **Validate Complete System**
   - Full automation recording
   - Playback accuracy
   - Mix file save/load
   - Integration testing

## 📁 Project Structure

```
EagleAutomation/
├── EagleAutomation.sln              # Visual Studio solution
├── README.md                         # This file
├── .gitignore                        # Git ignore rules
├── EagleAutomation/
│   ├── App.xaml                      # Application entry
│   ├── App.xaml.cs
│   ├── MainWindow.xaml               # Main console window
│   ├── MainWindow.xaml.cs
│   ├── appsettings.json              # Configuration
│   │
│   ├── Controllers/
│   │   ├── SerialController.cs       # RS-232 communication
│   │   └── ProtocolHandler.cs        # ⚠️ PROTOCOL COMMANDS GO HERE
│   │
│   ├── Models/
│   │   ├── AutomationMode.cs         # Enums and data models
│   │   └── Models.cs
│   │
│   ├── Views/
│   │   ├── MixEditWindow.xaml        # Mix editing window
│   │   ├── EventsWindow.xaml         # Events/timeline window
│   │   ├── TrackSheetWindow.xaml     # Track sheet window
│   │   ├── PreferencesWindow.xaml    # Settings window
│   │   └── *.xaml.cs                 # Code-behind files
│   │
│   └── Resources/
│       └── Styles.xaml               # Dark theme styling
│
└── Installer/
    └── Setup.iss                     # Inno Setup installer script
```

## 🔑 Key Files for Protocol Implementation

### **`Controllers/ProtocolHandler.cs`** ⚠️ IMPORTANT

This is where you'll add the reverse-engineered protocol. Currently contains placeholder implementations.

**Methods to implement:**
- `BuildAutomationModeCommand()` - Set READ/WRITE/UPDATE/etc.
- `BuildFaderCommand()` - Set fader position
- `BuildSwitchCommand()` - Control MUTE/SOLO/EQ
- `ParseIncomingData()` - Parse console responses

**Implementation steps:**
1. Analyze Portmon captures
2. Document protocol structure
3. Replace placeholder code with actual protocol
4. Test against real console

### **`Controllers/SerialController.cs`**

Handles serial port communication. Generally works as-is, but you may need to adjust:
- Timeout values
- Error handling
- Buffer sizes
- Flow control

## 🎨 User Interface

### Main Console Window
- 32 channel strips with faders
- Automation mode buttons
- Time code display
- Transport controls
- Status indicators

### Preferences Window
- COM port selection
- Serial parameters
- Time code configuration
- File path settings

### Other Windows (Placeholders)
- Mix Edit - Automation curve editing
- Events - Timeline events/cues
- Track Sheet - Session documentation

## 🐛 Debugging

### Serial Communication

Enable debug logging in `SerialController.cs` - all sent/received data is logged to Debug output:

```csharp
System.Diagnostics.Debug.WriteLine($"Sent: {BitConverter.ToString(command)}");
System.Diagnostics.Debug.WriteLine($"Received: {BitConverter.ToString(buffer)}");
```

View in Visual Studio: **View → Output → Show output from: Debug**

### Testing Without Console

The application will attempt to connect to the console on startup. If no console is connected, you'll get a warning but the app will still open, allowing UI development and testing.

## 📦 Building Installer

### Using Inno Setup (Recommended)

1. **Install Inno Setup:**
   - Download from https://jrsoftware.org/isinfo.php
   - Free and open source

2. **Build Release Version:**
   ```
   dotnet publish -c Release -r win-x64 --self-contained
   ```

3. **Run Inno Setup Compiler:**
   - Open `Installer/Setup.iss`
   - Click "Compile"
   - Installer will be created in `Installer/Output/`

4. **Distribute:**
   - Single `.exe` installer
   - ~100MB size (includes .NET runtime)
   - Standard Windows installation experience

## 🤝 Contributing

This is a personal project to reverse engineer vintage audio equipment. Contributions welcome!

Areas needing help:
- Protocol analysis expertise
- Additional UI features
- Testing with real hardware
- Documentation

## 📜 License

[Add your license here - MIT, GPL, etc.]

## 🙏 Acknowledgments

- **Otari Corporation** - Original Status 18R console and Eagle Automation software
- **Windows 95/98 Community** - Keeping vintage systems alive
- **Professional Audio Community** - For continued support of classic equipment

## 📞 Contact

[Add your contact information or GitHub profile]

---

## ⚠️ Important Notes

### Safety

- **Vintage Equipment:** The Status 18R is valuable vintage equipment. Test carefully!
- **Backup Mixes:** Always keep backups of automation data
- **Win95 System:** Keep the original Win95 system as a fallback

### Legal

- This is a clean-room reverse engineering project
- No original Eagle software code is used
- Protocol is being reverse engineered through observation only
- For personal/educational use with owned equipment

### Hardware Requirements

The Status 18R console requires:
- Proper power and grounding
- Calibrated audio paths
- Regular maintenance
- Professional installation

This software controls automation only - audio signal path is all analog hardware.

---

**Current Version:** 1.0.0 (Pre-Release - Protocol Reverse Engineering Phase)

**Last Updated:** October 2025
