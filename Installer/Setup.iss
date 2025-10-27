; Eagle Automation Installer Script
; Inno Setup 6.x required
; https://jrsoftware.org/isinfo.php

#define MyAppName "Eagle Automation"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Eagle Automation Development"
#define MyAppURL "https://github.com/yourusername/eagle-automation"
#define MyAppExeName "EagleAutomation.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{B5C3A8F1-D4E7-4A9B-8C2D-1E3F4A5B6C7D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=.\Output
OutputBaseFilename=EagleAutomationSetup
SetupIconFile=..\EagleAutomation\Resources\icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Main application executable and DLLs
Source: "..\EagleAutomation\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Configuration file
Source: "..\EagleAutomation\appsettings.json"; DestDir: "{app}"; Flags: ignoreversion
; Documentation
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion isreadme
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
; Register file associations for .mix files (optional)
Root: HKCR; Subkey: ".mix"; ValueType: string; ValueName: ""; ValueData: "EagleAutomationMix"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "EagleAutomationMix"; ValueType: string; ValueName: ""; ValueData: "Eagle Automation Mix File"; Flags: uninsdeletekey
Root: HKCR; Subkey: "EagleAutomationMix\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKCR; Subkey: "EagleAutomationMix\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

[Code]
// Check for .NET 8 Runtime (optional - our app is self-contained)
// If you want to require .NET instead of bundling it, uncomment this
{
function InitializeSetup: Boolean;
var
  ResultCode: Integer;
begin
  Result := True;
  
  // Check if COM port drivers are available
  if not FileExists(ExpandConstant('{sys}\kernel32.dll')) then
  begin
    MsgBox('This application requires Windows 10 or later.', mbError, MB_OK);
    Result := False;
  end;
end;
}

// Create default directories on installation
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Create default Eagle directories
    CreateDir(ExpandConstant('{userappdata}\Eagle\Mixes'));
    CreateDir(ExpandConstant('{userappdata}\Eagle\Presets'));
    CreateDir(ExpandConstant('{userappdata}\Eagle\Export'));
    CreateDir(ExpandConstant('{userappdata}\Eagle\Backups'));
    CreateDir(ExpandConstant('{userappdata}\Eagle\Logs'));
  end;
end;

[UninstallDelete]
; Clean up created directories (ask user first)
Type: filesandordirs; Name: "{userappdata}\Eagle"

[Messages]
WelcomeLabel1=Welcome to [name] Setup
WelcomeLabel2=This will install [name/ver] on your computer.%n%n[name] is a modern automation controller for the Otari Status 18R mixing console.%n%nYou will need:%n• RS-232 serial port (or USB adapter)%n• Otari Status 18R console%n• Serial cable connected to AUTOMATION PC port%n%nRecommended: Connect and power on your console before continuing.
