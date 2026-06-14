using System.Text.Json;

namespace lpastlai;

public class HotkeySettings
{
    public uint Modifiers { get; set; } = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;
    public uint Key { get; set; } = NativeMethods.VK_V;

    public string Display
    {
        get
        {
            string m = NativeMethods.ModifierString(Modifiers);
            string k = NativeMethods.KeyName(Key);
            return m.Length > 0 ? $"{m}+{k}" : k;
        }
    }

    private static readonly string Path_ =
        System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "lpastlai", "hotkey.json");

    public static HotkeySettings Load()
    {
        try
        {
            if (!File.Exists(Path_))
                return new HotkeySettings();
            var json = File.ReadAllText(Path_);
            return JsonSerializer.Deserialize<HotkeySettings>(json) ?? new HotkeySettings();
        }
        catch
        {
            return new HotkeySettings();
        }
    }

    public void Save()
    {
        try
        {
            var dir = System.IO.Path.GetDirectoryName(Path_)!;
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path_, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
        }
    }
}
