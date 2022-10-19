#define AppName "Deep Rock Galactic Launcher"
#define AppFileName "DeepRockLauncher.exe"
#define AppVersion "1.7"
#define AppPublisher "Deep Rock Galactic Launcher"

[Setup]
ExtraDiskSpaceRequired=2000000


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)

; Finish with a single }
AppId={{04888F49-F9DD-4A9F-A0F5-E9110DFB2A39}
OutputBaseFilename=DRGLauncherInstaller


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={commonpf}\{#AppName}
DefaultGroupName={#AppPublisher}
OutputDir=..\..
SolidCompression=yes
PrivilegesRequired=Admin
DisableWelcomePage=no
LicenseFile=/License.txt
Compression=lzma2/ultra
InternalCompressLevel=ultra
RestartIfNeededByRun=False
AllowUNCPath=False
AllowNoIcons=yes
;Prevent installer from being run multiple times in parallel
SetupMutex=SetupMutex{#SetupSetting("AppId")}
DisableDirPage=no

[CustomMessages]
LaunchProgram=Start Deep Rock Galactic Launcher after finishing installation

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppFileName}"; WorkingDir: "{app}"
Name: "{group}\Uninstall {#AppName}"; Filename: "{app}\unins000.exe"; WorkingDir: "{app}"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppFileName}"; WorkingDir: "{app}"

[Run]
Filename: "{app}\{#AppFileName}"; Description: {cm:LaunchProgram,{#AppName}}; Flags: nowait postinstall skipifsilent

[Files]
Source: "..\..\src\DeepRockLauncher.WPF\bin\Debug\net6.0-windows\*"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
