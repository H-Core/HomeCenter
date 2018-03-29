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
                ShowException(exception);
            }
        }

        public static void ShowException(Exception exception)
        {
            MessageBox.Show(exception.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
