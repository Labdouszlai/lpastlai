using System.Text.Json;
using System.Text.Json.Serialization;

namespace lpastlai;

public class AppSettings
{
    public uint HotkeyModifiers { get; set; } = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;
    public uint HotkeyKey { get; set; } = NativeMethods.VK_V;
    public int MaxTextItems { get; set; } = 8;
    public int MaxImageItems { get; set; } = 8;
    public int BgColorArgb { get; set; } = Color.FromArgb(30, 30, 30).ToArgb();
    public int FgColorArgb { get; set; } = Color.FromArgb(220, 220, 220).ToArgb();

    [JsonIgnore]
    public Color BgColor
    {
        get => Color.FromArgb(BgColorArgb);
        set => BgColorArgb = value.ToArgb();
    }

    [JsonIgnore]
    public Color FgColor
    {
        get => Color.FromArgb(FgColorArgb);
        set => FgColorArgb = value.ToArgb();
    }

    public string HotkeyDisplay
    {
        get
        {
            string m = NativeMethods.ModifierString(HotkeyModifiers);
            string k = NativeMethods.KeyName(HotkeyKey);
            return m.Length > 0 ? $"{m}+{k}" : k;
        }
    }

    private static readonly string Folder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "L'Pastlai");
    private static readonly string FilePath = Path.Combine(Folder, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new AppSettings();
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Folder);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
        }
    }
}
