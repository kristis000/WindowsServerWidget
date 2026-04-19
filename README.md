# Server Status

Small Windows tray app that monitors a server by TCP reachability and shows:
- tray status with red/green icon
- Windows toast on state change
- editable server name, host, port, and timeout
- auto-start on Windows login

## Build

Requirements:
- .NET SDK 7+
- Windows desktop runtime

Build from the repo root:

```powershell
dotnet build
```

Build outputs go under `.artifacts/`.

## Run

Launch the built `ServerStatus.exe` from:

```text
.artifacts/bin/Debug/net7.0-windows10.0.18362.0/
```

The app starts in the tray. Open settings from the tray menu or by double-clicking the tray icon.

## Notes

- The app follows the Windows light/dark app theme.
- Settings are stored under the current user's AppData profile.
- This repo ignores build artifacts and keeps all active outputs under `.artifacts/`.
