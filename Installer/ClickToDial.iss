#preproc ispp

#define ApplicationName "FairManager Click-to-Dial"
#define ApplicationVersion GetFileVersion( "..\bin\dial.exe" )
#define ApplicationGuid "7811ec0b-a46e-4f34-af45-09923c5e9489"
#define Now GetDateTimeString("yyyymmdd_hhnnss", "", "");

[Setup]
AppId={#ApplicationGuid}
AppName={#ApplicationName}
AppVersion={#ApplicationVersion}
UninstallDisplayName={#ApplicationName} {#ApplicationVersion}
AppPublisher="HARTWIG Communication & Events GmbH & Co. KG"
AppPublisherURL="https://fairmanager.de"
DefaultDirName={pf}\FairManager\{#ApplicationName}
DefaultGroupName={#ApplicationName}
UninstallDisplayIcon={app}\dial.exe
OutputBaseFilename=click-to-dial_{#ApplicationVersion}-x86

AlwaysRestart=no
AllowNetworkDrive=no
AllowUNCPath=no
AllowRootDirectory=no

[Files]
Source: "..\bin\dial.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\dial.exe.config"; DestDir: "{app}";

Source: "..\bin\ITapi3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\ITapi3.xml"; DestDir: "{app}";

Source: "..\bin\log4net.config"; DestDir: "{app}";
Source: "..\bin\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\log4net.xml"; DestDir: "{app}";

Source: "..\bin\setup.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\setup.exe.config"; DestDir: "{app}";

[Run]
Filename: {app}\setup.exe; Description: "Register protocol handler"; StatusMsg: "Registering protocol handler..."; Flags: postinstall runascurrentuser runhidden

[UninstallRun]
Filename: {app}\setup.exe; Parameters: "--uninstall"; StatusMsg: "Registering protocol handler..."
