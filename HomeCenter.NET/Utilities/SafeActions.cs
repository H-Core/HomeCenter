using System;
using System.Windows;

namespace HomeCenter.NET.Utilities
{
    public static class SafeActions
    {
        public static void Run(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
