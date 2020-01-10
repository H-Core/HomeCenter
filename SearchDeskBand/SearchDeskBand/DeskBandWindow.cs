using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using H.NET.Storages;

namespace H.NET.SearchDeskBand
{
    public partial class DeskBandWindow : Form
    {
        #region Constants

        public new const string CompanyName = "HomeCenter.NET";

        #endregion

        #region Events

        public event EventHandler<Exception> ExceptionOccurred;
        public event EventHandler<string> CommandSent;

        private void OnExceptionOccurred(Exception exception)
        {
            ExceptionOccurred?.Invoke(this, exception);
        }

        private void OnCommandSent(string command)
        {
            CommandSent?.Invoke(this, command);
        }

        #endregion

        #region Constructors

        public DeskBandWindow()
        {
            InitializeComponent();

            #region Auto Complete

            var storage = new CommandsStorage(CompanyName);
            storage.Load();

            var collection = new AutoCompleteStringCollection();
            collection.AddRange(storage.Select(i => i.Key).ToArray());
            TextBox.AutoCompleteCustomSource = collection;

            #endregion

            UpdateHistory();
        }

        #endregion

        #region Private methods

        private void UpdateHistory()
        {
            try
            {
                historyListBox.Items.Clear();

                var history = new CommandsHistory(CompanyName).Load();
                foreach (var command in history)
                {
                    historyListBox.Items.Add(command);
                }
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void DeskBandWindow_Deactivate(object sender, EventArgs e)
        {
            try
            {
                Hide();
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void DeskBandWindow_Activated(object sender, EventArgs e)
        {
            try
            {
                UpdateHistory();

                TextBox.Focus();
                //deskBandControl1.Focus();
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async Task RunAsync(string command, CancellationToken cancellationToken = default)
        {
            try
            {
                OnCommandSent(command);
                Hide();

                await Task.Delay(1000, cancellationToken); // TODO: fix

                UpdateHistory();
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    return;
                }

                var command = TextBox.Text;

                TextBox.Clear();

                await RunAsync(command);
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox.Focus();
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void HistoryListBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var item = historyListBox.SelectedItem as string;

                await RunAsync(item);
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void ClearHistoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                new CommandsHistory(CompanyName).Clear();
                UpdateHistory();
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        #endregion
    }
}
