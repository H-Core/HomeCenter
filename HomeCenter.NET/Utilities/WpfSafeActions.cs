using System;
using System.Windows;

namespace HomeCenter.NET.Utilities
{
    public static class WpfSafeActions
    {
        public static void Initialize(Action<Exception> action = null)
        {
            SafeActions.DefaultExceptionAction = action ?? ShowException;
            AppDomain.CurrentDomain.UnhandledException += (o, args) =>
                SafeActions.OnUnhandledException(args.ExceptionObject);
            Application.Current.DispatcherUnhandledException += (o, args) =>
            {
                args.Handled = true;
                SafeActions.OnUnhandledException(args.Exception);
            };
        }

        private static void ShowException(Exception exception)
        {
            MessageBox.Show(
                exception.ToString(),
                "Exception:",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
