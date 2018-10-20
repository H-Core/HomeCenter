using System.Windows;

namespace HomeCenter.NET.Views
{
    public partial class RectangleView
    {
        public RectangleView()
        {
            InitializeComponent();

            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }
    }
}
