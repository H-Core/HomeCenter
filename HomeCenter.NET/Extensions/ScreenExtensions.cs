using System;
using System.Collections.Generic;
using Caliburn.Micro;
using HomeCenter.NET.ViewModels.Utilities;

namespace HomeCenter.NET.Extensions
{
    // TODO: may be as IWindowManager extensions?
    public static class ScreenExtensions
    {
        public static IWindowManager WindowManager => IoC.Get<IWindowManager>();
        public static object GetInstance(Type type) => IoC.GetInstance(type, null);

        public static void ShowWindow<T>(this Screen screen, IDictionary<string, object> settings = null)
        {
            var instance = GetInstance(typeof(T));
            if (instance == null)
            {
                throw new ArgumentException($"Cannot find type: {typeof(T)}");
            }

            WindowManager.ShowWindow(instance, null, settings);
        }

        public static bool? ShowDialog<T>(this Screen screen, IDictionary<string, object> settings = null)
        {
            var instance = GetInstance(typeof(T));
            if (instance == null)
            {
                throw new ArgumentException($"Cannot find type: {typeof(T)}");
            }

            return WindowManager.ShowDialog(instance, null, settings);
        }

        public static void ShowWindow(this Screen screen, Screen rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            WindowManager.ShowWindow(rootModel, context, settings);
        }

        public static bool? ShowDialog(this Screen screen, Screen rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return WindowManager.ShowDialog(rootModel, context, settings);
        }

        public static void ShowMessageBox(this Screen screen, string text, string title = null)
        {
            var model = new MessageBoxViewModel(text, title);

            screen.ShowWindow(model);
        }

    }
}
