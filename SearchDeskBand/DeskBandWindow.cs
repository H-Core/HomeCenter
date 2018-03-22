using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using H.Storages;

namespace SearchDeskBand
{
    public partial class DeskBandWindow : Form
    {
        public DeskBandWindow()
        {
            InitializeComponent();

            #region Auto Complete

            var storage = new CommandsStorage();
            storage.Load();

            var collection = new AutoCompleteStringCollection();
            collection.AddRange(storage.Select(i => i.Key).ToArray());
            TextBox.AutoCompleteCustomSource = collection;

            #endregion

            UpdateHistory();
        }

        private void UpdateHistory()
        {
            historyListBox.Items.Clear();

            var history = CommandsHistory.Load();
            history.Reverse();
            foreach (var command in history)
            {
                historyListBox.Items.Add(command);
            }
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

        private async void Run(string command)
        {
            CreateNewCommandFile(command);
            Hide();

            await Task.Delay(1000); // TODO: fix

            UpdateHistory();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            Run(TextBox.Text);
            TextBox.Clear();
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            TextBox.Focus();
        }

        private void HistoryListBox_DoubleClick(object sender, EventArgs e)
        {
            var item = historyListBox.SelectedItem as string;

            Run(item);
        }
    }
}
