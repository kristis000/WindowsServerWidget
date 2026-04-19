# Server Status

Small Windows tray app that monitors a server by TCP reachability and shows:
- tray status with red/green icon
- Windows toast on state change
- editable server name, host, port, and timeout
- auto-start on Windows login

## Build

Requirements:
- .NET SDK 10+
- Windows desktop runtime

Build from the repo root:

```powershell
dotnet build ServerStatus.sln
```

Build outputs go under `.artifacts/`.

### Standard Build

Command:

```powershell
dotnet build ServerStatus.sln
```

Output:

```text
.artifacts/bin/
```

Notes:
- this is the normal development build
- the full output folder is required, not just `ServerStatus.exe`
- the target machine needs the .NET 10 desktop runtime installed

### Single-File Publish

Command:

```powershell
dotnet publish .\src\ServerStatus.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=true /p:DebugType=None /p:DebugSymbols=false -o .artifacts\publish\single-file-sc-compressed
```

Output:

```text
.artifacts/publish/single-file-sc-compressed/ServerStatus.exe
```

Notes:
- creates a single self-contained executable for distribution
- the target machine does not need .NET 10 installed

## Layout

```text
src/
  Assets/         app icon and manifest
  Configuration/  settings and startup registration
  Interop/        native Win32/DWM interop
  Models/         small app/domain types
  Services/       probing, theme, notifications
  UI/             tray/menu/settings UI
  Program.cs
  ServerStatus.csproj
```

## Run

Launch the built `ServerStatus.exe` from:

```text
.artifacts/bin/
```

The app starts in the tray. Open settings from the tray menu or by double-clicking the tray icon.

## Notes

- The app follows the Windows light/dark app theme.
- Settings are stored under the current user's AppData profile.
- Auto-start uses the current user's Windows `Run` key entry at logon.
