using System;
using System.Drawing;
using System.Windows.Forms;

namespace H.NET.SearchDeskBand
{
    public partial class DeskBandControl : UserControl, IDisposable
    {
        #region Properties

        private DeskBandWindow Window { get; } = new DeskBandWindow();

        #endregion

        public DeskBandControl()
        {
            InitializeComponent();

            Window.VisibleChanged += (sender, args) => Label.Visible = !Window.Visible;
        }

        #region Event handlers

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

        #endregion

        #region IDisposable

        public new void Dispose()
        {
            Window.Dispose();
            base.Dispose();
        }

        #endregion
    }
}
