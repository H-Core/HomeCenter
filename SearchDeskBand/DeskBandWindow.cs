using System;
using System.IO;
using System.Windows.Forms;

namespace SearchDeskBand
{
    public partial class DeskBandWindow : Form
    {
        public DeskBandWindow()
        {
            InitializeComponent();
        }

        private void DeskBandWindow_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }

        private void DeskBandWindow_Activated(object sender, EventArgs e)
        {
            TextBox.Focus();
            //deskBandControl1.Focus();
        }

        private static string SharedDirectory => Directory.CreateDirectory(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "HomeCenter.NET",
                "commands")
        ).FullName;

        private static void CreateNewCommandFile(string message)
        {
            var fileName = $"{new Random().Next()}.txt";
            var path = Path.Combine(SharedDirectory, fileName);

            File.WriteAllText(path, message);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            CreateNewCommandFile(TextBox.Text);
            TextBox.Clear();
            Hide();
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            TextBox.Focus();
        }
    }
}
