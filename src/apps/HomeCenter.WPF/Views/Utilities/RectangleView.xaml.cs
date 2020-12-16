using System.Windows;

namespace HomeCenter.NET.Views.Utilities
{
    public partial class RectangleView
    {
        public RectangleView()
        {
            InitializeComponent();

            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
        }
    }
}
