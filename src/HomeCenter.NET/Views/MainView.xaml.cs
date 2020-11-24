using System;
using System.Windows;

namespace HomeCenter.NET.Views
{
    public partial class MainView : IDisposable
    {
        public MainView()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            if (TaskbarIcon == null)
            {
                return;
            }

            TaskbarIcon.Icon = null;
            TaskbarIcon.Visibility = Visibility.Hidden;
            TaskbarIcon.Dispose();
            TaskbarIcon = null;
        }
    }
}