using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace lpastlai;

public class PastePopupForm : Form
{
    private readonly ListBox listBox;
    private readonly List<ClipItem> items;
    private readonly Action<string> onSelect;
    private const int ItemHeight = 44;
    private const int MaxPreviewLen = 60;

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWCP_ROUNDSMALL = 4;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public PastePopupForm(List<ClipItem> items, Action<string> onSelect)
    {
        this.items = items;
        this.onSelect = onSelect;

        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        int count = items.Count == 0 ? 1 : Math.Min(items.Count, 10);
        Size = new Size(420, count * ItemHeight);
        MinimumSize = new Size(280, ItemHeight);
        BackColor = Color.FromArgb(30, 30, 30);

        _ = Handle;

        listBox = new ListBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            IntegralHeight = false,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.FromArgb(220, 220, 220),
            Font = new Font("Segoe UI", 9f),
            ItemHeight = ItemHeight,
            DrawMode = DrawMode.OwnerDrawFixed
        };

        if (items.Count == 0)
        {
            listBox.Items.Add("(empty)");
            listBox.Enabled = false;
        }
        else
        {
            foreach (var item in items)
                listBox.Items.Add(item);
        }

        listBox.DrawItem += OnDrawItem;
        listBox.DoubleClick += (_, _) => SelectCurrent();
        listBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { SelectCurrent(); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) Close();
        };

        Controls.Add(listBox);

        Deactivate += (_, _) => Close();
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
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

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (listBox.Items.Count > 0)
            listBox.SelectedIndex = 0;
        listBox.Focus();

        var screen = Screen.FromPoint(Location).WorkingArea;
        int x = Math.Clamp(Location.X, screen.Left, screen.Right - Width);
        int y = Math.Clamp(Location.Y, screen.Top, screen.Bottom - Height);
        Location = new Point(x, y);

        FadeIn();
    }

    private async void FadeIn()
    {
        try
        {
            for (int i = 0; i < 8; i++)
            {
                Opacity = (i + 1) / 8.0;
                await Task.Delay(12);
            }
        }
        catch
        {
            Opacity = 1;
        }
    }

    private void OnDrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var rect = e.Bounds;
        bool selected = (e.State & DrawItemState.Selected) != 0;

        using var bg = new SolidBrush(selected ? Color.FromArgb(70, 70, 75) : BackColor);
        g.FillRectangle(bg, rect);

        if (items.Count == 0)
            return;

        var item = (ClipItem?)listBox.Items[e.Index];
        if (item == null) return;

        using var sepPen = new Pen(Color.FromArgb(42, 42, 42));
        g.DrawLine(sepPen, rect.Left, rect.Top, rect.Right, rect.Top);

        using var previewFont = new Font("Segoe UI", 9.5f);
        using var textBrush = new SolidBrush(selected ? Color.White : Color.FromArgb(220, 220, 220));
        string preview = BuildPreview(item.Text);
        var textRect = new Rectangle(rect.Left + 12, rect.Top + 6, rect.Width - 24, 20);
        g.DrawString(preview, previewFont, textBrush, textRect);

        using var timeFont = new Font("Segoe UI", 8f);
        using var timeBrush = new SolidBrush(Color.FromArgb(130, 130, 130));
        string timeStr = FormatTime(item.Time);
        var timeRect = new Rectangle(rect.Left + 12, rect.Bottom - 18, rect.Width - 24, 14);
        g.DrawString(timeStr, timeFont, timeBrush, timeRect);
    }

    private static string FormatTime(DateTime dt)
    {
        var diff = DateTime.Now - dt;
        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalDays < 1) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        return dt.ToString("MMM dd");
    }

    private static string BuildPreview(string text)
    {
        string preview = text.Replace("\r", " ").Replace("\n", " ¶ ").Trim();
        return preview.Length > MaxPreviewLen ? preview[..MaxPreviewLen] + "…" : preview;
    }

    private void SelectCurrent()
    {
        int idx = listBox.SelectedIndex;
        if (idx >= 0 && idx < items.Count)
        {
            string text = items[idx].Text;
            Close();
            onSelect(text);
        }
    }
}
