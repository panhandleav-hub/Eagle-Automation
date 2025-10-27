# Eagle Automation - Build Instructions

## Prerequisites

- **Windows 10/11** (WPF is Windows-only)
- **.NET 8.0 SDK** - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
- **Visual Studio 2022** (optional, but recommended) - Community Edition is free

## Quick Start - Visual Studio

1. Transfer this entire `EagleAutomation` folder to your Windows machine
2. Double-click `EagleAutomation.sln` to open in Visual Studio
3. Press **F5** to build and run
4. The application will compile and launch automatically

## Quick Start - Command Line

```bash
cd EagleAutomation\EagleAutomation
dotnet restore
dotnet build
dotnet run
```

## Creating a Standalone Executable

Run the included batch file:

```bash
build-release.bat
```

This will create a self-contained executable in the `publish` folder that includes all dependencies. You can distribute this folder to other Windows machines without requiring .NET installation.

## Configuration

On first run, the application will create `appsettings.json` with default settings:

- **Serial Port**: COM1 (change to match your Otari console connection)
- **Baud Rate**: 19200
- **Channel Count**: 32

Edit `appsettings.json` or use the Preferences window to configure these settings.

## Serial Port Setup

1. Connect your Otari Status 18R console via RS-232 to your Windows PC
2. Note the COM port number (check Device Manager > Ports)
3. Update the COM port in Preferences window or `appsettings.json`
4. Click "Connect to Console" in the main window

## Troubleshooting

### "Cannot find COM port"
- Verify the Otari console is connected and powered on
- Check Device Manager for the correct COM port number
- Update the port name in Preferences

### "Access denied to COM port"
- Close any other applications using the serial port
- Run the application as Administrator
- Check that no other instance of Eagle Automation is running

### Build errors
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`

## Protocol Capture

The protocol implementation is currently a placeholder. To capture the real Otari protocol:

1. Run the original Eagle software on Windows
2. Use **Portmon** (Sysinternals) to capture serial communication
3. Replace placeholder protocol in `Controllers/OtariProtocol.cs`

## Logging

Application logs are written to:
- `logs/eagle-automation-YYYYMMDD.log` - General application logs
- `logs/protocol-capture-YYYYMMDD.log` - Serial communication capture

## Windows Navigation

- **Console** - Main channel strip view (32 channels with faders, mute, solo, EQ)
- **Mix Edit** - Automation curve editing and timeline
- **Events** - Timeline events, cues, switches, and snapshots
- **Track Sheet** - Session documentation with channel labels and notes
- **Preferences** - Serial port and application settings

## Next Steps

1. Test the GUI and verify all windows display correctly
2. Connect to your Otari console
3. Capture the real protocol communication
4. Replace placeholder protocol implementation
5. Test automation features with actual hardware

Good luck with your Otari Status 18R automation project!
