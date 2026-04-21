using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeepOpen
{
    public class DarkMessageBox : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static DialogResult Show(string text, string caption)
        {
            using var form = new DarkMessageBox(text, caption);
            return form.ShowDialog();
        }

        private DarkMessageBox(string text, string caption)
        {
            this.Text = caption;
            this.Size = new Size(420, 220);
            this.BackColor = Color.FromArgb(24, 24, 24);
            this.ForeColor = Color.FromArgb(230, 230, 230);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            this.Font = new Font("Segoe UI Variable Small", 10);

            // Enable dark title bar for Windows 10/11
            try
            {
                int darkMode = 1;
                DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
            }
            catch { /* Ignore if not supported */ }

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var label = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI Variable Display", 11, FontStyle.Regular),
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(32, 32, 32)
            };

            var yesButton = CreateButton("Yes", DialogResult.Yes, Color.FromArgb(0, 103, 192), Color.FromArgb(0, 120, 212));
            var noButton = CreateButton("No", DialogResult.No, Color.FromArgb(60, 60, 60), Color.FromArgb(80, 80, 80));

            // Position buttons on the right
            yesButton.Location = new Point(300, 15);
            noButton.Location = new Point(200, 15);

            buttonPanel.Controls.Add(yesButton);
            buttonPanel.Controls.Add(noButton);

            mainPanel.Controls.Add(label);
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);

            this.AcceptButton = yesButton;
            this.CancelButton = noButton;
        }

        private Button CreateButton(string text, DialogResult result, Color backColor, Color hoverColor)
        {
            var btn = new Button
            {
                Text = text,
                DialogResult = result,
                Size = new Size(90, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Variable Small", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;
            
            return btn;
        }
    }
}
