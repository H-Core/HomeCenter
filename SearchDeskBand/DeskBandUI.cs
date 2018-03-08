using System.Diagnostics;
using System.Windows.Forms;

namespace SearchDeskBand
{
    public partial class DeskBandControl : UserControl
    {
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

            Process.Start("http://google.com#q=" + textBoxSearch.Text);
            textBoxSearch.Clear();
        }
    }
}
