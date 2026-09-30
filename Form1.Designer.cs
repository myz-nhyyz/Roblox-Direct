// language: C#, file: Form1.Designer.cs
using System.Drawing;
using System.Windows.Forms;

namespace RobloxDirect
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox logoPictureBox;
        private Label titleLabel;
        private Label subtitleLabel;
        private Label linkLabel;
        private TextBox linkTextBox;
        private Button pasteButton;
        private Button joinButton;
        private ComboBox clientComboBox;
        private Label statusLabel;
        private Panel inputPanel;
        private Button themeToggleButton;
        private Panel introPanel;
        private Label introTitleLabel;
        private Label introSubtitleLabel;
        private Button closeButton;
        private TextBox customPathTextBox;
        private Button customBrowseButton;
        private Panel settingsPanel;
        private CheckBox startupCheckBox;
        private CheckBox minimizedCheckBox;
        private Label customLabel;
        private Label settingsLabel;
        private Label clientLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            logoPictureBox = new PictureBox();
            titleLabel = new Label();
            subtitleLabel = new Label();
            linkLabel = new Label();
            linkTextBox = new TextBox();
            pasteButton = new Button();
            joinButton = new Button();
            clientComboBox = new ComboBox();
            statusLabel = new Label();
            inputPanel = new Panel();
            themeToggleButton = new Button();
            introPanel = new Panel();
            introTitleLabel = new Label();
            introSubtitleLabel = new Label();
            closeButton = new Button();
            customPathTextBox = new TextBox();
            customBrowseButton = new Button();
            settingsPanel = new Panel();
            startupCheckBox = new CheckBox();
            minimizedCheckBox = new CheckBox();
            customLabel = new Label();
            settingsLabel = new Label();
            clientLabel = new Label();
            inputPanel.SuspendLayout();
            introPanel.SuspendLayout();
            settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            SuspendLayout();

            logoPictureBox.BackColor = Color.Transparent;
            logoPictureBox.Location = new Point(42, 26);
            logoPictureBox.Size = new Size(58, 58);
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(24, 31, 42);
            titleLabel.Location = new Point(112, 8);
            titleLabel.Text = "Join Roblox trực tiếp";

            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 10.5F);
            subtitleLabel.ForeColor = Color.FromArgb(92, 101, 116);
            subtitleLabel.Location = new Point(114, 57);
            subtitleLabel.Text = "Dán link game hoặc link invite để mở thẳng trong ứng dụng Roblox.";

            themeToggleButton.FlatAppearance.BorderSize = 1;
            themeToggleButton.FlatStyle = FlatStyle.Flat;
            themeToggleButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            themeToggleButton.Location = new Point(612, 38);
            themeToggleButton.Size = new Size(86, 33);
            themeToggleButton.Text = "☀️ Sáng";
            themeToggleButton.UseVisualStyleBackColor = false;
            themeToggleButton.Click += ThemeToggleButton_Click;

            closeButton.FlatAppearance.BorderSize = 1;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            closeButton.Location = new Point(706, 12);
            closeButton.Size = new Size(28, 28);
            closeButton.Text = "×";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += CloseButton_Click;

            introPanel.BackColor = Color.FromArgb(2, 5, 10);
            introPanel.BorderStyle = BorderStyle.None;
            introPanel.Controls.Add(introTitleLabel);
            introPanel.Controls.Add(introSubtitleLabel);
            introPanel.Location = new Point(0, 0);
            introPanel.Size = new Size(760, 560);
            introPanel.Visible = true;

            introTitleLabel.AutoSize = false;
            introTitleLabel.Font = new Font("Segoe UI Semibold", 28F, FontStyle.Bold);
            introTitleLabel.ForeColor = Color.White;
            introTitleLabel.Location = new Point(0, 135);
            introTitleLabel.Size = new Size(760, 52);
            introTitleLabel.Text = "RobloxDirect";
            introTitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            introSubtitleLabel.AutoSize = false;
            introSubtitleLabel.Font = new Font("Segoe UI", 11F);
            introSubtitleLabel.ForeColor = Color.FromArgb(170, 180, 190);
            introSubtitleLabel.Location = new Point(0, 192);
            introSubtitleLabel.Size = new Size(760, 28);
            introSubtitleLabel.Text = "Launching...";
            introSubtitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            linkLabel.AutoSize = true;
            linkLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            linkLabel.ForeColor = Color.FromArgb(52, 61, 76);
            linkLabel.BackColor = Color.Transparent;
            linkLabel.Location = new Point(50, 118);
            linkLabel.Size = new Size(150, 18);
            linkLabel.Text = "LINK ROBLOX";

            inputPanel.BackColor = Color.White;
            inputPanel.BorderStyle = BorderStyle.None;
            inputPanel.Controls.Add(linkTextBox);
            inputPanel.Controls.Add(pasteButton);
            inputPanel.Location = new Point(46, 140);
            inputPanel.Size = new Size(652, 54);

            linkTextBox.BorderStyle = BorderStyle.None;
            linkTextBox.Font = new Font("Segoe UI", 11F);
            linkTextBox.Location = new Point(16, 16);
            linkTextBox.Size = new Size(500, 20);
            linkTextBox.KeyDown += LinkTextBox_KeyDown;

            pasteButton.BackColor = Color.FromArgb(239, 243, 248);
            pasteButton.FlatAppearance.BorderSize = 0;
            pasteButton.FlatStyle = FlatStyle.Flat;
            pasteButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            pasteButton.Location = new Point(532, 10);
            pasteButton.Size = new Size(108, 32);
            pasteButton.Text = "Dán link";
            pasteButton.UseVisualStyleBackColor = false;
            pasteButton.Click += PasteButton_Click;

            joinButton.BackColor = Color.FromArgb(31, 106, 220);
            joinButton.FlatAppearance.BorderSize = 0;
            joinButton.FlatStyle = FlatStyle.Flat;
            joinButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            joinButton.ForeColor = Color.White;
            joinButton.Location = new Point(46, 208);
            joinButton.Size = new Size(652, 52);
            joinButton.Text = "Mở trong Roblox";
            joinButton.UseVisualStyleBackColor = false;
            joinButton.Click += JoinButton_Click;

            clientComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            clientComboBox.Font = new Font("Segoe UI", 10F);
            clientComboBox.FormattingEnabled = true;

            clientLabel.AutoSize = true;
            clientLabel.BackColor = Color.Transparent;
            clientLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            clientLabel.ForeColor = Color.FromArgb(52, 61, 76);
            clientLabel.Location = new Point(50, 276);
            clientLabel.Text = "CLIENT ROBLOX";

            clientComboBox.Location = new Point(46, 298);
            clientComboBox.Size = new Size(652, 25);

            customLabel.AutoSize = true;
            customLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            customLabel.ForeColor = Color.FromArgb(52, 61, 76);
            customLabel.BackColor = Color.Transparent;
            customLabel.Location = new Point(50, 338);
            customLabel.Size = new Size(190, 18);
            customLabel.Text = "CUSTOM ROBLOX PATH";

            customPathTextBox.BorderStyle = BorderStyle.None;
            customPathTextBox.Font = new Font("Segoe UI", 10F);
            customPathTextBox.Location = new Point(46, 362);
            customPathTextBox.Size = new Size(552, 20);

            customBrowseButton.BackColor = Color.FromArgb(239, 243, 248);
            customBrowseButton.FlatAppearance.BorderSize = 0;
            customBrowseButton.FlatStyle = FlatStyle.Flat;
            customBrowseButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            customBrowseButton.Location = new Point(610, 356);
            customBrowseButton.Size = new Size(88, 32);
            customBrowseButton.Text = "Chọn";
            customBrowseButton.UseVisualStyleBackColor = false;
            customBrowseButton.Click += ChooseCustomPathButton_Click;

            settingsLabel.AutoSize = true;
            settingsLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            settingsLabel.ForeColor = Color.FromArgb(52, 61, 76);
            settingsLabel.BackColor = Color.Transparent;
            settingsLabel.Location = new Point(50, 410);
            settingsLabel.Size = new Size(100, 18);
            settingsLabel.Text = "SETTINGS";

            settingsPanel.BackColor = Color.FromArgb(15, 20, 28);
            settingsPanel.BorderStyle = BorderStyle.None;
            settingsPanel.Location = new Point(46, 432);
            settingsPanel.Size = new Size(652, 68);

            startupCheckBox.AutoSize = true;
            startupCheckBox.Font = new Font("Segoe UI", 10F);
            startupCheckBox.Location = new Point(12, 14);
            startupCheckBox.Text = "Load on system startup";
            startupCheckBox.CheckedChanged += StartupCheckBox_CheckedChanged;

            minimizedCheckBox.AutoSize = true;
            minimizedCheckBox.Font = new Font("Segoe UI", 10F);
            minimizedCheckBox.Location = new Point(12, 38);
            minimizedCheckBox.Text = "Start minimized";
            minimizedCheckBox.CheckedChanged += StartMinimizedCheckBox_CheckedChanged;

            settingsPanel.Controls.Add(startupCheckBox);
            settingsPanel.Controls.Add(minimizedCheckBox);

            statusLabel.AutoSize = false;
            statusLabel.Font = new Font("Segoe UI", 9.5F);
            statusLabel.ForeColor = Color.FromArgb(92, 101, 116);
            statusLabel.Location = new Point(50, 510);
            statusLabel.Size = new Size(630, 44);
            statusLabel.Text = "RobloxDirect không mở website; link được chuyển thẳng sang app Roblox.";

            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 249, 252);
            ClientSize = new Size(760, 560);
            Controls.Add(introPanel);
            Controls.Add(closeButton);
            Controls.Add(statusLabel);
            Controls.Add(clientComboBox);
            Controls.Add(clientLabel);
            Controls.Add(customBrowseButton);
            Controls.Add(customPathTextBox);
            Controls.Add(customLabel);
            Controls.Add(settingsPanel);
            Controls.Add(settingsLabel);
            Controls.Add(joinButton);
            Controls.Add(inputPanel);
            Controls.Add(linkLabel);
            Controls.Add(subtitleLabel);
            Controls.Add(titleLabel);
            Controls.Add(logoPictureBox);
            Controls.Add(themeToggleButton);
            linkLabel.BringToFront();
            clientLabel.BringToFront();
            customLabel.BringToFront();
            settingsLabel.BringToFront();
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "RobloxDirect";
            inputPanel.ResumeLayout(false);
            inputPanel.PerformLayout();
            introPanel.ResumeLayout(false);
            introPanel.PerformLayout();
            settingsPanel.ResumeLayout(false);
            settingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}