using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCenter.NET.Utilities
{
    public static class SafeActions
    {
        public static Action<Exception> DefaultExceptionAction { get; set; } = 
            e => Console.WriteLine($@"Exception: {e.ToString()}");

        public static void Run(Action action, Action<Exception> extencionAction = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception exception)
            {
                if (extencionAction != null)
                {
                    extencionAction(exception);
                }
                else
                {
                    DefaultExceptionAction?.Invoke(exception);
                }
            }
        }

        public static void OnUnhandledException(object exceptionObject, Action<Exception> extencionAction = null)
        {
            if (!(exceptionObject is Exception exception))
            {
                exception = new NotSupportedException($"Unhandled exception doesn't derive from System.Exception: {exceptionObject}");
            }

            // Ignore ThreadAbortException
            if (exception is ThreadAbortException)
            {
                return;
            }

            if (extencionAction != null)
            {
                extencionAction(exception);
            }
            else
            {
                DefaultExceptionAction?.Invoke(exception);
            }
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
