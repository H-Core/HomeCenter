using System.Windows;

namespace HomeCenter.NET.Views.Utilities
{
    public partial class PopupView
    {
        public PopupView()
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
