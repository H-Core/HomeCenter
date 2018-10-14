using System.Windows;

namespace HomeCenter.NET.Views
{
    public partial class PopUpView
    {
        public PopUpView()
        {
            InitializeComponent();

            MaxWidth = SystemParameters.WorkArea.Width / 4;
            MaxHeight = SystemParameters.WorkArea.Height / 2;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Width - ActualWidth;
            Top = SystemParameters.WorkArea.Height - ActualHeight;
        }
    }
}
