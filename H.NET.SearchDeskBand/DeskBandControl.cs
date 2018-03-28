using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using H.NET.Core.Utilities;
using H.NET.Utilities;

namespace H.NET.SearchDeskBand
{
    public partial class DeskBandControl : UserControl, IDisposable
    {
        #region Properties

        private DeskBandWindow Window { get; } = new DeskBandWindow();
        private Server Server { get; } = new Server(Options.IpcPortToDeskBand);
        private Dictionary<string, Action<string>> ActionDictionary { get; } = new Dictionary<string, Action<string>>();

        #endregion

        #region Constructors

        public DeskBandControl()
        {
            InitializeComponent();

            AddAction("start", message => RecordButton.BackColor = Color.RoyalBlue);
            AddAction("stop", message => RecordButton.BackColor = DefaultBackColor);

            Window.VisibleChanged += (sender, args) => Label.Visible = !Window.Visible;

            Server.NewMessage += OnNewMessage;
        }

        #endregion

        #region Event handlers

        private void OnNewMessage(string message)
        {
            var values = message.SplitOnlyFirst(' ');
            if (!ActionDictionary.TryGetValue(values[0], out var action))
            {
                return;
            }

            action?.Invoke(values[1]);
        }

        private void OnClick(object sender, EventArgs e)
        {
            Window.Visible = !Window.Visible;
            var location = PointToScreen(Point.Empty);
            Window.Location = location;
            Window.Top -= Window.Height;
            Window.Top += Height;
            Window.Left -= 1; // border
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            DeskBandWindow.SendCommand("show-commands");
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            DeskBandWindow.SendCommand("start-record");
        }

        #endregion

        #region IDisposable

        public new void Dispose()
        {
            Window.Dispose();
            base.Dispose();
        }

        #endregion

        #region Private methods

        public void AddAction(string key, Action<string> action)
        {
            ActionDictionary[key.ToLowerInvariant()] = action;
        }

        #endregion
    }
}
