using System.Runtime.InteropServices;

namespace lpastlai;

internal static class NativeMethods
{
    public const int WM_CLIPBOARDUPDATE = 0x031D;
    public const int WM_HOTKEY = 0x0312;

    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;
    public const uint MOD_NOREPEAT = 0x4000;

    public const uint VK_V = 0x56;
    public const byte VK_CONTROL = 0x11;
    public const uint KEYEVENTF_KEYUP = 0x0002;

    public static readonly Dictionary<uint, string> KeyNames = new()
    {
        { 0x41, "A" }, { 0x42, "B" }, { 0x43, "C" }, { 0x44, "D" },
        { 0x45, "E" }, { 0x46, "F" }, { 0x47, "G" }, { 0x48, "H" },
        { 0x49, "I" }, { 0x4A, "J" }, { 0x4B, "K" }, { 0x4C, "L" },
        { 0x4D, "M" }, { 0x4E, "N" }, { 0x4F, "O" }, { 0x50, "P" },
        { 0x51, "Q" }, { 0x52, "R" }, { 0x53, "S" }, { 0x54, "T" },
        { 0x55, "U" }, { 0x56, "V" }, { 0x57, "W" }, { 0x58, "X" },
        { 0x59, "Y" }, { 0x5A, "Z" },
        { 0x30, "0" }, { 0x31, "1" }, { 0x32, "2" }, { 0x33, "3" },
        { 0x34, "4" }, { 0x35, "5" }, { 0x36, "6" }, { 0x37, "7" },
        { 0x38, "8" }, { 0x39, "9" },
        { 0x70, "F1" }, { 0x71, "F2" }, { 0x72, "F3" }, { 0x73, "F4" },
        { 0x74, "F5" }, { 0x75, "F6" }, { 0x76, "F7" }, { 0x77, "F8" },
        { 0x78, "F9" }, { 0x79, "F10" }, { 0x7A, "F11" }, { 0x7B, "F12" },
        { 0x6A, "*" }, { 0x6B, "+" }, { 0x6D, "-" }, { 0x6F, "/" },
        { 0xBC, "," }, { 0xBE, "." }, { 0xBA, ";" }, { 0xDB, "[" },
        { 0xDD, "]" }, { 0xC0, "`" }, { 0xDE, "'" }, { 0xBF, "/" },
        { 0x09, "Tab" }, { 0x20, "Space" }, { 0x1B, "Esc" },
    };

    public static string ModifierString(uint mods)
    {
        var parts = new List<string>();
        if ((mods & MOD_CONTROL) != 0) parts.Add("Ctrl");
        if ((mods & MOD_ALT) != 0) parts.Add("Alt");
        if ((mods & MOD_SHIFT) != 0) parts.Add("Shift");
        if ((mods & MOD_WIN) != 0) parts.Add("Win");
        return string.Join("+", parts);
    }

    public static string KeyName(uint vk) =>
        KeyNames.TryGetValue(vk, out var n) ? n : $"VK_{vk:X02}";

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
}
