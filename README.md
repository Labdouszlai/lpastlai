# lpastlai

A lightweight clipboard history app for Windows. Lives in the tray, remembers your copied items (text + images), and lets you paste any of them with a configurable hotkey.

![](https://img.shields.io/badge/Windows-10%2B-blue)
![](https://img.shields.io/badge/.NET-8.0-purple)
![](https://img.shields.io/github/license/labdouszlai/lpastlai)

## How it works

- Runs in the system tray.
- Saves text and images you copy to `%AppData%\lpastlai\history.json`.
- Hit the hotkey anywhere and a split popup appears where your cursor is.
- Left side: text items. Right side: image items with thumbnails.
- Tab to switch panels, arrow keys + Enter to paste. Esc to dismiss.

## Download

Pre-built binaries are in [Releases](https://github.com/labdouszlai/lpastlai/releases):

| Platform | File |
|----------|------|
| Windows 64-bit | `lpastlai-win-x64.exe` |
| Windows 32-bit | `lpastlai-win-x86.exe` |

No install needed. Right-click the tray icon for settings and "Start with Windows".

## Settings

Right-click the tray icon → **Settings** to configure:

| Setting | Description |
|---------|-------------|
| Hotkey | Modifiers + key for the paste popup |
| Max text items | How many text items to show |
| Max image items | How many image items to show |
| Background color | Popup background color |
| Font color | Popup text color |

## Building from source

```powershell
git clone https://github.com/labdouszlai/lpastlai
cd lpastlai
dotnet build -c Release
```

Or use `build.ps1` to produce both win-x64 and win-x86 single-file executables.

## License

MIT © [labdouszlai](https://github.com/labdouszlai)
