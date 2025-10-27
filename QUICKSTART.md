# Quick Start Guide

Get Eagle Automation up and running in 5 minutes!

## ⚡ Fast Setup

### 1. Install Visual Studio 2022

Download and install **Visual Studio 2022 Community** (free):
- https://visualstudio.microsoft.com/downloads/
- Select workload: ".NET desktop development"
- Includes .NET 8.0 SDK

### 2. Clone and Open Project

```bash
git clone <your-repo-url>
cd EagleAutomation
```

Double-click `EagleAutomation.sln` to open in Visual Studio.

### 3. Build and Run

Press `F5` to build and run!

The application will:
- Attempt to connect to console on COM1
- Show a connection warning if no console found
- Open the main window ready for development

## 🔌 Connecting Your Console

### Hardware Setup

1. **Console:** Power on your Otari Status 18R
2. **Cable:** Connect RS-232 cable:
   - Console side: "AUTOMATION PC" port (rear panel)
   - PC side: COM port or USB-to-serial adapter
3. **Verify:** Check Windows Device Manager for COM port number

### Software Configuration

1. Launch Eagle Automation
2. Click **Preferences** button
3. Switch to **Settings** tab
4. Select your COM port from dropdown
5. Click **Test Connection**
6. Status should show "Connected"

## 🎛️ Using the Interface

### Main Console Window

- **Channel Strips** - Shows 32 channels (visual mockup)
- **Automation Modes** - Click READ/WRITE/UPDATE etc.
- **Time Code Display** - Shows running time code
- **Transport** - Play/Stop controls

### Navigation

- **Console** - Main fader/automation view
- **Mix Edit** - Coming soon
- **Events** - Coming soon
- **Track Sheet** - Coming soon
- **Preferences** - Settings and configuration

## 🔧 Next Steps: Protocol Implementation

### Phase 1: Capture Serial Data (CRITICAL)

You need a Win95 PC with:
1. Original Eagle software installed
2. Portmon running (serial monitor)
3. Console connected

**Capture Data For:**
- Clicking each automation mode button
- Moving each fader
- Pressing MUTE/SOLO buttons
- All console operations

Save all captures - this is the KEY to making the software work!

### Phase 2: Analyze Protocol

Look at the captured hex data:
- What bytes are sent for READ mode?
- What bytes are sent for WRITE mode?
- How is fader position encoded?
- What's the message structure?

### Phase 3: Implement in Code

Edit `EagleAutomation/Controllers/ProtocolHandler.cs`:

```csharp
public static byte[] BuildAutomationModeCommand(AutomationMode mode)
{
    // Replace this placeholder with actual protocol!
    return mode switch
    {
        AutomationMode.Read => new byte[] { 0xF0, 0x10, 0x01, 0xF7 }, // Example
        AutomationMode.Write => new byte[] { 0xF0, 0x10, 0x02, 0xF7 },
        // ... add real commands from your captures
    };
}
```

### Phase 4: Test Against Console

1. Build and run application
2. Click automation mode button
3. Verify console responds correctly
4. Repeat for each command
5. Document what works!

## 📋 Development Checklist

- [ ] Visual Studio 2022 installed
- [ ] Project builds without errors
- [ ] Application launches successfully
- [ ] COM ports detected in preferences
- [ ] Win95 PC ready for serial capture
- [ ] Portmon installed and configured
- [ ] Console connected and powered on
- [ ] Ready to capture protocol data!

## 🆘 Troubleshooting

### "Cannot connect to console"

- Check COM port selection in Preferences
- Verify cable is connected to "AUTOMATION PC" port
- Check Windows Device Manager for COM port
- Try different COM port numbers

### "Application won't build"

- Ensure .NET 8.0 SDK is installed
- Right-click solution → "Restore NuGet Packages"
- Clean and rebuild solution
- Check Output window for specific errors

### "No COM ports available"

- If using USB-to-serial adapter, install drivers
- Check Device Manager → Ports (COM & LPT)
- Some laptops have no built-in serial ports (normal)
- USB adapters work fine!

### "Faders don't move on console"

- This is expected! Protocol not implemented yet
- You need to capture and implement the protocol first
- See Phase 1-4 above

## 🎯 Success Criteria

You're ready to proceed when:

✅ Application builds and runs  
✅ Can select COM port in preferences  
✅ Win95 PC ready with Portmon  
✅ Console responds to original Eagle software  
✅ Ready to capture serial protocol data

---

## 📚 Additional Resources

- **Full Documentation:** See README.md
- **Code Structure:** See project structure in README
- **Protocol Notes:** See ProtocolHandler.cs comments
- **UI Styling:** See Resources/Styles.xaml

## 💡 Tips

- Start simple - get one command working first
- Test frequently against real console
- Keep Win95 system as reference
- Document everything you discover
- Take screenshots of Portmon captures
- Back up your mix files!

---

**Ready to start capturing protocol data? Let's go! 🚀**

For questions or issues, see the main README.md or open a GitHub issue.
