using System.Runtime.InteropServices;

namespace lpastlai;

public class SettingsForm : Form
{
    private readonly CheckBox chkCtrl, chkAlt, chkShift, chkWin;
    private readonly Label keyDisplay;
    private readonly AppSettings settings;
    private readonly NumericUpDown numText, numImg;
    private readonly Panel bgPanel, fgPanel;
    private bool capturing;

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWCP_ROUNDSMALL = 4;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public AppSettings Result { get; private set; }

    public SettingsForm(AppSettings current)
    {
        settings = new AppSettings
        {
            HotkeyModifiers = current.HotkeyModifiers,
            HotkeyKey = current.HotkeyKey,
            MaxTextItems = current.MaxTextItems,
            MaxImageItems = current.MaxImageItems,
            BgColorArgb = current.BgColorArgb,
            FgColorArgb = current.FgColorArgb,
        };
        Result = current;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(380, 420);
        BackColor = Color.FromArgb(30, 30, 30);
        Icon = Program.LoadAppIcon();
        _ = Handle;

        var title = new Label
        {
            Text = "Settings",
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Segoe UI Semibold", 10f),
            ForeColor = Color.FromArgb(180, 180, 180),
            BackColor = Color.FromArgb(38, 38, 38),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0)
        };

        var wrap = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(14, 10, 14, 10),
            BackColor = Color.FromArgb(30, 30, 30),
            AutoScroll = true
        };

        AddSectionHeader(wrap, "Hotkey");

        wrap.Controls.Add(MakeLabel("Modifiers:"));
        chkCtrl = MakeCheck("Ctrl");
        chkAlt = MakeCheck("Alt");
        chkShift = MakeCheck("Shift");
        chkWin = MakeCheck("Win");
        var mods = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            Size = new Size(340, 24),
            BackColor = Color.Transparent
        };
        mods.Controls.AddRange(new Control[] { chkCtrl, chkAlt, chkShift, chkWin });
        wrap.Controls.Add(mods);

        var keyLabel = MakeLabel("Key:");
        keyLabel.Margin = new Padding(0, 6, 0, 0);
        wrap.Controls.Add(keyLabel);

        keyDisplay = new Label
        {
            Text = NativeMethods.KeyName(settings.HotkeyKey),
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 55),
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(120, 32),
            Cursor = Cursors.Hand
        };
        keyDisplay.Click += (_, _) => StartCapture();
        wrap.Controls.Add(keyDisplay);

        AddSectionHeader(wrap, "Display");

        numText = MakeNud(wrap, "Max text items:", settings.MaxTextItems, 1, 50, 0, 10);
        numImg = MakeNud(wrap, "Max image items:", settings.MaxImageItems, 1, 50, 0, 6);

        AddSectionHeader(wrap, "Theme");

        var bgRow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            Size = new Size(340, 28),
            BackColor = Color.Transparent,
            Margin = new Padding(0, 0, 0, 4)
        };
        var bgLabel = MakeLabel("Background:");
        bgLabel.Size = new Size(130, 22);
        bgPanel = MakeColorPanel(settings.BgColor, OnBgClick);
        bgRow.Controls.Add(bgLabel);
        bgRow.Controls.Add(bgPanel);
        wrap.Controls.Add(bgRow);

        var fgRow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            Size = new Size(340, 28),
            BackColor = Color.Transparent,
            Margin = new Padding(0, 0, 0, 10)
        };
        var fgLabel = MakeLabel("Font color:");
        fgLabel.Size = new Size(130, 22);
        fgPanel = MakeColorPanel(settings.FgColor, OnFgClick);
        fgRow.Controls.Add(fgLabel);
        fgRow.Controls.Add(fgPanel);
        wrap.Controls.Add(fgRow);

        var btnRow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Size = new Size(340, 32),
            BackColor = Color.Transparent
        };
        var saveBtn = new Button
        {
            Text = "Save Settings",
            Font = new Font("Segoe UI", 9f),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(0, 103, 192),
            ForeColor = Color.White,
            FlatAppearance = { BorderSize = 0 },
            Size = new Size(80, 30)
        };
        saveBtn.Click += (_, _) => SaveAndClose();
        var cancelBtn = new Button
        {
            Text = "Cancel",
            Font = new Font("Segoe UI", 9f),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(50, 50, 55),
            ForeColor = Color.FromArgb(180, 180, 180),
            FlatAppearance = { BorderSize = 0 },
            Size = new Size(80, 30),
            Margin = new Padding(0, 0, 8, 0)
        };
        cancelBtn.Click += (_, _) => Close();
        btnRow.Controls.Add(saveBtn);
        btnRow.Controls.Add(cancelBtn);
        wrap.Controls.Add(btnRow);

        Controls.Add(wrap);
        Controls.Add(title);

        KeyPreview = true;
        KeyDown += OnKeyDown;
        SyncChecks();
    }

    private static void AddSectionHeader(FlowLayoutPanel parent, string text)
    {
        parent.Controls.Add(new Label
        {
            Text = text,
            Font = new Font("Segoe UI Semibold", 9.5f),
            ForeColor = Color.FromArgb(0, 130, 220),
            BackColor = Color.Transparent,
            Size = new Size(340, 20),
            Margin = new Padding(0, 8, 0, 2)
        });
    }

    private static Label MakeLabel(string text)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(180, 180, 180),
            Size = new Size(340, 18)
        };
    }

    private static NumericUpDown MakeNud(FlowLayoutPanel parent, string label, int value, int min, int max, int topMargin, int bottomMargin)
    {
        var row = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            Size = new Size(340, 28),
            BackColor = Color.Transparent,
            Margin = new Padding(0, topMargin, 0, bottomMargin)
        };
        row.Controls.Add(new Label
        {
            Text = label,
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(180, 180, 180),
            Size = new Size(200, 22)
        });
        var nud = new NumericUpDown
        {
            Value = value,
            Minimum = min,
            Maximum = max,
            Width = 60,
            BackColor = Color.FromArgb(50, 50, 55),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        row.Controls.Add(nud);
        parent.Controls.Add(row);
        return nud;
    }

    private static Panel MakeColorPanel(Color c, EventHandler onClick)
    {
        var p = new Panel
        {
            Size = new Size(100, 22),
            BackColor = c,
            Cursor = Cursors.Hand,
            BorderStyle = BorderStyle.FixedSingle
        };
        p.Click += onClick;
        return p;
    }

    private void OnBgClick(object? sender, EventArgs e)
    {
        using var dlg = new ColorDialog { Color = settings.BgColor, AnyColor = true, FullOpen = true };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            settings.BgColor = dlg.Color;
            bgPanel.BackColor = dlg.Color;
        }
    }

    private void OnFgClick(object? sender, EventArgs e)
    {
        using var dlg = new ColorDialog { Color = settings.FgColor, AnyColor = true, FullOpen = true };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            settings.FgColor = dlg.Color;
            fgPanel.BackColor = dlg.Color;
        }
    }

    private CheckBox MakeCheck(string text)
    {
        return new CheckBox
        {
            Text = text,
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(200, 200, 200),
            BackColor = Color.Transparent,
            AutoSize = true
        };
    }

    private void StartCapture()
    {
        capturing = true;
        keyDisplay.Text = "... press a key";
        keyDisplay.ForeColor = Color.FromArgb(0, 180, 255);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!capturing) return;
        if (e.KeyCode == Keys.Escape)
        {
            capturing = false;
            keyDisplay.Text = NativeMethods.KeyName(settings.HotkeyKey);
            keyDisplay.ForeColor = Color.White;
            return;
        }
        uint vk = (uint)e.KeyCode;
        settings.HotkeyKey = vk;
        keyDisplay.Text = NativeMethods.KeyName(vk);
        keyDisplay.ForeColor = Color.White;
        capturing = false;
    }

    private void SyncChecks()
    {
        chkCtrl.Checked = (settings.HotkeyModifiers & NativeMethods.MOD_CONTROL) != 0;
        chkAlt.Checked = (settings.HotkeyModifiers & NativeMethods.MOD_ALT) != 0;
        chkShift.Checked = (settings.HotkeyModifiers & NativeMethods.MOD_SHIFT) != 0;
        chkWin.Checked = (settings.HotkeyModifiers & NativeMethods.MOD_WIN) != 0;
    }

    private void SaveAndClose()
    {
        uint mods = 0;
        if (chkCtrl.Checked) mods |= NativeMethods.MOD_CONTROL;
        if (chkAlt.Checked) mods |= NativeMethods.MOD_ALT;
        if (chkShift.Checked) mods |= NativeMethods.MOD_SHIFT;
        if (chkWin.Checked) mods |= NativeMethods.MOD_WIN;
        if (mods == 0)
        {
            keyDisplay.Text = "pick at least one modifier";
            return;
        }
        settings.HotkeyModifiers = mods | NativeMethods.MOD_NOREPEAT;
        settings.MaxTextItems = (int)numText.Value;
        settings.MaxImageItems = (int)numImg.Value;
        settings.Save();
        Result = settings;
        Close();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ClassStyle |= 0x00020000;
            return cp;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (Environment.OSVersion.Version.Build >= 22000)
        {
            int useDark = 1;
            DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
            int corner = DWMWCP_ROUNDSMALL;
            DwmSetWindowAttribute(Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref corner, sizeof(int));
        }
    }
}
