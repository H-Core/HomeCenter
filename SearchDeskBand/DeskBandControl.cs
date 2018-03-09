using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SearchDeskBand
{
    public partial class DeskBandControl : UserControl, IDisposable
    {
        private static string SharedDirectory => Directory.CreateDirectory(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "HomeCenter.NET",
                "commands")
            ).FullName;

        private DeskBandWindow Window { get; } = new DeskBandWindow();

        private static void CreateNewCommandFile(string message)
        {
            var fileName = $"{new Random().Next()}.txt";
            var path = Path.Combine(SharedDirectory, fileName);

            File.WriteAllText(path, message);
        }

        public DeskBandControl()
        {
            InitializeComponent();
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            CreateNewCommandFile(TextBox.Text);
            TextBox.Clear();
        }

        private void TextBoxSearch_Click(object sender, EventArgs e)
        {
            Window.Visible = !Window.Visible;
            var location = PointToScreen(Point.Empty);
            Window.Location = location;
            Window.Top -= Window.Height;

            TextBox.Focus();
        }

        public new void Dispose()
        {
            Window.Dispose();
            base.Dispose();
        }
    }
}
