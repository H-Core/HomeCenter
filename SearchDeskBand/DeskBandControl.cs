using System;
using System.IO;
using System.Windows.Forms;

namespace SearchDeskBand
{
    public partial class DeskBandControl : UserControl
    {
        public static string SharedDirectory => Directory.CreateDirectory(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "HomeCenter.NET",
                "commands")
            ).FullName;

        public static void CreateNewCommandFile(string message)
        {
            var fileName = $"{new Random().Next()}.txt";
            var path = Path.Combine(SharedDirectory, fileName);

            File.WriteAllText(path, message);
        }

        public DeskBandControl()
        {
            InitializeComponent();
        }

        private void textBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            CreateNewCommandFile(textBoxSearch.Text);
            textBoxSearch.Clear();
        }
    }
}
