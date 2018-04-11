using System;
using System.Threading.Tasks;
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

        public static async Task SafeAction(Func<Task> action, Action<Exception> exceptionAction, Func<bool> check = null, Action finallyAction = null)
        {
            try
            {
                if (check != null && !check())
                {
                    return;
                }

                if (action != null)
                    await action.Invoke();
            }
            catch (Exception exception)
            {
                exceptionAction?.Invoke(exception);
            }
            finally
            {
                try
                {
                    finallyAction?.Invoke();
                }
                catch (Exception exception)
                {
                    exceptionAction?.Invoke(exception);
                }
            }
        }

        public static void SafeAction(Action action, Action<Exception> exceptionAction, Func<bool> check = null, Action finallyAction = null)
        {
            try
            {
                if (check != null && !check())
                {
                    return;
                }

                action?.Invoke();
            }
            catch (Exception exception)
            {
                exceptionAction?.Invoke(exception);
            }
            finally
            {
                try
                {
                    finallyAction?.Invoke();
                }
                catch (Exception exception)
                {
                    exceptionAction?.Invoke(exception);
                }
            }
        }

        public static EventHandler<T> CreateSafeEventHandler<T>(Action<Exception, object, T> exceptionAction, EventHandler<T> action)
        {
            return (sender, args) =>
            {
                try
                {
                    action?.Invoke(sender, args);
                }
                catch (Exception exception)
                {
                    exceptionAction?.Invoke(exception, sender, args);
                }
            };
        }
    }
}
