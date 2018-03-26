using System;
using System.Drawing;
using System.Windows.Forms;
using H.NET.SearchDeskBand;

namespace SearchDeskBand
{
    public partial class DeskBandControl : UserControl, IDisposable
    {
        private DeskBandWindow Window { get; } = new DeskBandWindow();

        public DeskBandControl()
        {
            InitializeComponent();

            Window.VisibleChanged += (sender, args) => Label.Visible = !Window.Visible;
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

        public new void Dispose()
        {
            Window.Dispose();
            base.Dispose();
        }
    }
}
