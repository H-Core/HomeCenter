using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using HomeCenter.NET.ViewModels.Utilities;

namespace HomeCenter.NET.Extensions
{
    // TODO: may be as IWindowManager extensions?
    public static class ScreenExtensions
    {
        public static IWindowManager WindowManager => IoC.Get<IWindowManager>();
        public static object GetInstance(Type type) => IoC.GetInstance(type, null);

        public static async Task ShowWindowAsync<T>(this Screen screen, IDictionary<string, object> settings = null)
        {
            var instance = GetInstance(typeof(T));
            if (instance == null)
            {
                throw new ArgumentException($"Cannot find type: {typeof(T)}");
            }

            await WindowManager.ShowWindowAsync(instance, null, settings);
        }

        public static async Task<bool?> ShowDialogAsync<T>(this Screen screen, IDictionary<string, object> settings = null)
        {
            var instance = GetInstance(typeof(T));
            if (instance == null)
            {
                throw new ArgumentException($"Cannot find type: {typeof(T)}");
            }

            return await WindowManager.ShowDialogAsync(instance, null, settings);
        }

        public static async Task ShowWindowAsync(this Screen screen, Screen rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            await WindowManager.ShowWindowAsync(rootModel, context, settings);
        }

        public static async Task<bool?> ShowDialogAsync(this Screen screen, Screen rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return await WindowManager.ShowDialogAsync(rootModel, context, settings);
        }

        public static async Task ShowMessageBoxAsync(this Screen screen, string text, string title = null)
        {
            var model = new MessageBoxViewModel(text, title);

            await screen.ShowWindowAsync(model);
        }

    }
}
