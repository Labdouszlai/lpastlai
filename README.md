# lpastlai

A lightweight clipboard history app for Windows. Lives in the tray, remembers your last 30 copied items (text + images), and lets you paste any of them with **Ctrl+Shift+V**.

![](https://img.shields.io/badge/Windows-10%2B-blue)
![](https://img.shields.io/badge/.NET-8.0-purple)
![](https://img.shields.io/github/license/labdouszlai/lpastlai)

## How it works

- Runs in the system tray — no windows, no taskbar entry.
- Saves text and images you copy to `%AppData%\lpastlai\history.json`.
- Hit **Ctrl+Shift+V** anywhere and a split popup appears where your cursor is.
- Left side: text items. Right side: image items with thumbnails.
- Tab to switch panels, arrow keys + Enter to paste. Esc to dismiss.

## Why Ctrl+Shift+V?

Windows doesn't let apps inject items into another program's right-click menu — notepad, chrome, vscode all draw their own. The only thing that works *everywhere* is a global hotkey. That's what this is.

## Download

Pre-built binaries are in [Releases](https://github.com/labdouszlai/lpastlai/releases):

| Platform | File |
|----------|------|
| Windows 64-bit | `lpastlai-win-x64.exe` |
| Windows 32-bit | `lpastlai-win-x86.exe` |

No install needed — download and run. If you want it to start with Windows, right-click the tray icon and check "Start with Windows".

## Building from source

```powershell
# requires .NET 8 SDK
git clone https://github.com/labdouszlai/lpastlai
cd lpastlai
dotnet build -c Release
```

Output is in `bin\Release\net8.0-windows\lpastlai.exe`.

### Publishing a standalone exe

```powershell
# 64-bit (no .NET runtime needed on target PC)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# 32-bit
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true
```

Or use the included `build.ps1` which builds both in one go.

## Customization

Most tweaks are a constant change away:

| What | Where |
|------|-------|
| Hotkey | `HiddenHostForm.cs` — `NativeMethods.MOD_CONTROL \| MOD_SHIFT`, `VK_V` |
| History limit | `HistoryStore.cs` — `MaxItems` (default 30) |
| Preview length | `PastePopupForm.cs` — `MaxPreview` (default 60) |
| Popup width | `PastePopupForm.cs` — `Size(660, ...)` |

## License

MIT © [labdouszlai](https://github.com/labdouszlai)
