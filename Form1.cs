// language: C#, file: Form1.cs
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace RobloxDirect
{
    public partial class Form1 : Form
    {
        private readonly string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RobloxDirect",
            "settings.json");

        private bool isDarkTheme = true;
        private readonly Timer loadingTimer = new Timer();
        private int loadingDotIndex = 0;
        private AppSettings appSettings = new AppSettings();
        private bool isDragging;
        private Point dragStartPoint;
        private Point windowStartLocation;

        public Form1()
        {
            InitializeComponent();
            Icon = LoadAppIcon();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FormBorderStyle = FormBorderStyle.None;
            Padding = new Padding(10);
            this.Paint += Form1_Paint;
            this.Resize += (_, __) => Invalidate();
            this.Resize += (_, __) => ApplyRoundedWindowRegion();
            AddDragHandler(this);
            AddDragHandler(logoPictureBox);
            AddDragHandler(titleLabel);
            AddDragHandler(subtitleLabel);
            LoadSettings();
            LoadClients();
            ApplyTheme();
            ApplyStartupSetting();
            if (appSettings.StartMinimized)
                WindowState = FormWindowState.Minimized;
            StartIntroAnimation();
        }

        private void AddDragHandler(Control control)
        {
            control.MouseDown += DragSurface_MouseDown;
            control.MouseMove += DragSurface_MouseMove;
            control.MouseUp += DragSurface_MouseUp;
        }

        private void DragSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                Control surface = sender as Control ?? this;
                dragStartPoint = surface.PointToScreen(e.Location);
                windowStartLocation = Location;
            }
        }

        private void DragSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Control surface = sender as Control ?? this;
                Point screenPoint = surface.PointToScreen(e.Location);
                Location = new Point(
                    windowStartLocation.X + screenPoint.X - dragStartPoint.X,
                    windowStartLocation.Y + screenPoint.Y - dragStartPoint.Y);
            }
        }

        private void DragSurface_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isDragging = false;
        }

        private void ApplyRoundedWindowRegion()
        {
            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                using (GraphicsPath path = CreateRoundedRectangle(
                    new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1), 24))
                {
                    Region = new Region(path);
                }
            }
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                linkTextBox.Text = Clipboard.GetText().Trim();
                linkTextBox.SelectAll();
                linkTextBox.Focus();
            }
        }

        private void JoinButton_Click(object sender, EventArgs e)
        {
            OpenRobloxLink();
        }

        private void LinkTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                OpenRobloxLink();
            }
        }

        private void ThemeToggleButton_Click(object sender, EventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            ApplyTheme();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChooseCustomPathButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Chọn file Roblox.exe",
                Filter = "Roblox Executable (*.exe)|*.exe|All files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    customPathTextBox.Text = dialog.FileName;
                    appSettings.CustomExecutablePath = dialog.FileName;
                    SaveSettings();
                    LoadClients();
                }
            }
        }

        private void StartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            appSettings.LoadOnSystemStartup = startupCheckBox.Checked;
            SaveSettings();
            ApplyStartupSetting();
        }

        private void StartMinimizedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            appSettings.StartMinimized = minimizedCheckBox.Checked;
            SaveSettings();
        }

        private void StartIntroAnimation()
        {
            introPanel.Visible = false;
            loadingTimer.Interval = 320;
            loadingTimer.Tick += (_, __) =>
            {
                loadingDotIndex = (loadingDotIndex + 1) % 4;
                statusLabel.Text = "Đang chuẩn bị" + new string('.', loadingDotIndex + 1);
            };
            loadingTimer.Start();
        }

        private void OpenRobloxLink()
        {
            try
            {
                string robloxUri = RobloxLinkParser.ToRobloxUri(linkTextBox.Text);
                RobloxClient selectedClient = clientComboBox.SelectedItem as RobloxClient;
                if (selectedClient == null)
                    throw new RobloxLinkException("Chưa chọn client Roblox.");

                RobloxClient clientToLaunch;
                if (selectedClient.Id == "auto")
                {
                    var detected = RobloxClientLauncher.DetectClients();
                    clientToLaunch = null;
                    foreach (var c in detected)
                    {
                        if (c.Id != "default") { clientToLaunch = c; break; }
                    }
                    if (clientToLaunch == null)
                        clientToLaunch = new RobloxClient("default", "Roblox thường", null);
                }
                else if (selectedClient.Id == "custom")
                {
                    if (string.IsNullOrWhiteSpace(appSettings.CustomExecutablePath) ||
                        !File.Exists(appSettings.CustomExecutablePath))
                        throw new RobloxLinkException("Chưa chọn file Roblox.exe hợp lệ trong mục Custom.");

                    clientToLaunch = new RobloxClient("custom", "Custom", appSettings.CustomExecutablePath);
                }
                else
                {
                    clientToLaunch = RobloxClientLauncher.FindById(selectedClient.Id)
                        ?? new RobloxClient("default", "Roblox thường", null);
                }

                RobloxClientLauncher.Launch(clientToLaunch, robloxUri);
                statusLabel.ForeColor = Color.FromArgb(28, 125, 72);
                statusLabel.Text = "Đã gửi link tới " + clientToLaunch.DisplayName + ".";
                loadingTimer.Stop();
            }
            catch (RobloxLinkException exception)
            {
                statusLabel.ForeColor = Color.FromArgb(183, 74, 64);
                statusLabel.Text = exception.Message;
            }
            catch (Win32Exception)
            {
                statusLabel.ForeColor = Color.FromArgb(183, 74, 64);
                statusLabel.Text = "Không tìm thấy protocol Roblox. Hãy cài hoặc mở Roblox trước, rồi thử lại.";
            }
        }

        private void LoadClients()
        {
            clientComboBox.DisplayMember = "DisplayName";
            clientComboBox.Items.Clear();
            clientComboBox.Items.Add(new RobloxClient("auto", "Tự động", null));
            if (!string.IsNullOrWhiteSpace(appSettings.CustomExecutablePath))
                clientComboBox.Items.Add(new RobloxClient("custom", "Custom", appSettings.CustomExecutablePath));

            foreach (RobloxClient client in RobloxClientLauncher.DetectClients())
                clientComboBox.Items.Add(client);

            clientComboBox.SelectedIndex = 0;
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                try
                {
                    appSettings = JsonConvert.DeserializeObject<AppSettings>(
                        File.ReadAllText(settingsPath)) ?? new AppSettings();
                }
                catch
                {
                    appSettings = new AppSettings();
                }
            }

            customPathTextBox.Text = appSettings.CustomExecutablePath ?? string.Empty;
            startupCheckBox.Checked = appSettings.LoadOnSystemStartup;
            minimizedCheckBox.Checked = appSettings.StartMinimized;
        }

        private void SaveSettings()
        {
            try
            {
                string directory = Path.GetDirectoryName(settingsPath)
                    ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                Directory.CreateDirectory(directory);
                File.WriteAllText(settingsPath,
                    JsonConvert.SerializeObject(appSettings, Formatting.Indented));
            }
            catch { }
        }

        private void ApplyStartupSetting()
        {
            try
            {
                using (RegistryKey runKey = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run"))
                {
                    if (appSettings.LoadOnSystemStartup)
                        runKey.SetValue("RobloxDirect", "\"" + Application.ExecutablePath + "\"");
                    else
                        runKey.DeleteValue("RobloxDirect", false);
                }
            }
            catch { }
        }

        private void ApplyTheme()
        {
            Color background = isDarkTheme ? Color.FromArgb(8, 10, 15) : Color.FromArgb(244, 247, 250);
            Color panel = isDarkTheme ? Color.FromArgb(18, 22, 29) : Color.FromArgb(255, 255, 255);
            Color border = isDarkTheme ? Color.FromArgb(39, 46, 57) : Color.FromArgb(219, 226, 236);
            Color text = isDarkTheme ? Color.FromArgb(245, 247, 250) : Color.FromArgb(24, 31, 42);
            Color muted = isDarkTheme ? Color.FromArgb(154, 170, 184) : Color.FromArgb(92, 101, 116);
            Color accent = Color.FromArgb(53, 137, 255);
            Color buttonSecondary = isDarkTheme ? Color.FromArgb(24, 30, 39) : Color.FromArgb(238, 242, 247);
            Color hoverAccent = Color.FromArgb(40, 110, 225);

            BackColor = background;
            titleLabel.ForeColor = text;
            subtitleLabel.ForeColor = muted;
            linkLabel.ForeColor = text;
            inputPanel.BackColor = panel;
            inputPanel.BorderStyle = BorderStyle.None;
            linkTextBox.BackColor = panel;
            linkTextBox.ForeColor = text;
            linkTextBox.BorderStyle = BorderStyle.None;
            pasteButton.BackColor = buttonSecondary;
            pasteButton.ForeColor = text;
            pasteButton.FlatAppearance.BorderColor = border;
            joinButton.BackColor = accent;
            joinButton.ForeColor = Color.White;
            joinButton.FlatAppearance.BorderColor = hoverAccent;
            clientComboBox.BackColor = panel;
            clientComboBox.ForeColor = text;
            clientComboBox.FlatStyle = FlatStyle.Flat;
            statusLabel.ForeColor = muted;
            themeToggleButton.BackColor = panel;
            themeToggleButton.ForeColor = text;
            themeToggleButton.FlatAppearance.BorderColor = border;
            themeToggleButton.Text = isDarkTheme ? "☀️ Sáng" : "🌙 Tối";
            customPathTextBox.BackColor = panel;
            customPathTextBox.ForeColor = text;
            customBrowseButton.BackColor = buttonSecondary;
            customBrowseButton.ForeColor = text;
            startupCheckBox.ForeColor = text;
            minimizedCheckBox.ForeColor = text;
            customLabel.ForeColor = text;
            settingsLabel.ForeColor = text;
            clientLabel.ForeColor = text;
            settingsPanel.BackColor = panel;
            closeButton.BackColor = panel;
            closeButton.ForeColor = text;
            closeButton.FlatAppearance.BorderColor = border;

            logoPictureBox.Image = CreateLogoImage();
            introPanel.BackColor = Color.FromArgb(255, 2, 5, 10);
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = CreateRoundedRectangle(bounds, 24))
            using (LinearGradientBrush gradient = new LinearGradientBrush(
                bounds,
                isDarkTheme ? Color.FromArgb(9, 12, 18) : Color.FromArgb(248, 250, 252),
                isDarkTheme ? Color.FromArgb(14, 18, 27) : Color.FromArgb(255, 255, 255),
                LinearGradientMode.Vertical))
            using (Pen borderPen = new Pen(isDarkTheme ? Color.FromArgb(42, 52, 65) : Color.FromArgb(220, 228, 236), 1.25f))
            {
                e.Graphics.FillPath(gradient, path);
                e.Graphics.DrawPath(borderPen, path);
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Left, rect.Top, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Bitmap CreateLogoImage()
        {
            Bitmap bitmap = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(isDarkTheme ? Color.Black : Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(isDarkTheme ? Color.White : Color.Black))
                {
                    PointF[] leftWing =
                    {
                        new PointF(8, 10),
                        new PointF(17, 10),
                        new PointF(47, 54),
                        new PointF(38, 54)
                    };
                    PointF[] rightWing =
                    {
                        new PointF(47, 10),
                        new PointF(56, 10),
                        new PointF(26, 54),
                        new PointF(17, 54)
                    };
                    g.FillPolygon(brush, leftWing);
                    g.FillPolygon(brush, rightWing);
                    g.FillRectangle(brush, 8, 10, 9, 44);
                    g.FillRectangle(brush, 47, 10, 9, 44);
                }
            }
            return bitmap;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            introPanel.Visible = false;
        }

        private static Icon LoadAppIcon()
        {
            string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "favicon.ico");
            if (File.Exists(iconPath))
                return new Icon(iconPath);
            return null;
        }

        private sealed class AppSettings
        {
            public bool LoadOnSystemStartup { get; set; }
            public bool StartMinimized { get; set; }
            public string CustomExecutablePath { get; set; }
        }
    }
}