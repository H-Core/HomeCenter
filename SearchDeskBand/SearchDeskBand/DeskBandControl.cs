using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using H.NET.Core.Utilities;
using H.NET.Utilities;

namespace H.NET.SearchDeskBand
{
    public partial class DeskBandControl : UserControl, IDisposable
    {
        #region Constants

        public const string ApplicationName = "HomeCenter.NET";

        #endregion

        #region Properties

        private IpcService IpcService { get; }
        private DeskBandWindow Window { get; set; }
        private Dictionary<string, Action<string>> ActionDictionary { get; } = new Dictionary<string, Action<string>>();

        #endregion

        #region Constructors

        public DeskBandControl()
        {
            InitializeComponent();

            AddAction("start", message => RecordButton.BackColor = Color.RoyalBlue);
            AddAction("stop", message => RecordButton.BackColor = Color.White);

            Window = new DeskBandWindow();
            Window.ExceptionOccurred += (sender, exception) => OnExceptionOccurred(exception);
            Window.CommandSent += Window_OnCommandSent;
            Window.VisibleChanged += (sender, args) => Label.Visible = !Window.Visible;

            IpcService = new IpcService();
            IpcService.Connected += IpcService_OnConnected;
            IpcService.Disconnected += IpcService_OnDisconnected;
            IpcService.MessageReceived += (sender, text) => IpcService_OnMessageReceived(text);
            IpcService.ExceptionOccurred += (sender, exception) => OnExceptionOccurred(exception);
        }

        #endregion

        #region Event handlers

        private static void OnExceptionOccurred(Exception exception)
        {
            MessageBox.Show(exception.ToString(), @"Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void Window_OnCommandSent(object sender, string command)
        {
            try
            {
                await RunAsync(command);
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void IpcService_OnConnected(object sender, EventArgs e)
        {
            Label.ForeColor = Color.RoyalBlue;
        }

        private void IpcService_OnDisconnected(object sender, EventArgs e)
        {
            Label.ForeColor = Color.Gray;
        }

        private void IpcService_OnMessageReceived(string message)
        {
            try
            {
                var values = message.SplitOnlyFirst(' ');
                if (!ActionDictionary.TryGetValue(values[0], out var action))
                {
                    return;
                }

                action?.Invoke(values[1]);
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            try
            {
                Window.Visible = !Window.Visible;
                var location = PointToScreen(Point.Empty);
                Window.Location = location;
                Window.Top -= Window.Height;
                Window.Top += Height;
                Window.Left -= 1; // border
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void DeskBandControl_Load(object sender, EventArgs e)
        {
            try
            {
                await IpcService.ConnectAsync();
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void RecordButton_Click(object sender, EventArgs e)
        {
            try
            {
                await RunAsync("start-record");
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void UiButton_Click(object sender, EventArgs e)
        {
            try
            {
                await RunAsync("show-ui");
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void MenuButton_Click(object sender, EventArgs e)
        {
            try
            {
                await RunAsync("show-commands");
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async void SettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                await RunAsync("show-settings");
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        #endregion

        #region IDisposable

        public new void Dispose()
        {
            Window?.Dispose();
            Window = null;

            IpcService.DisposeAsync().AsTask().Wait();

            base.Dispose();
        }

        #endregion

        #region Private methods

        private async Task RunAsync(string command, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!Process.GetProcessesByName(ApplicationName).Any())
                {
                    var path = Startup.GetFilePath($"{ApplicationName}.exe");
                    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    {
                        Process.Start(path);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }

                await IpcService.WriteAsync(command, cancellationToken);
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private void AddAction(string key, Action<string> action)
        {
            try
            {
                ActionDictionary[key.ToLowerInvariant()] = action;
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        #endregion
    }
}
