﻿using System;
using System.Collections.Generic;
using Caliburn.Micro;
using HomeCenter.NET.ViewModels;

namespace HomeCenter.NET.Extensions
{
    // TODO: may be as IWindowManager extensions?
    public static class ScreenExtensions
    {
        public static IWindowManager WindowManager => IoC.Get<IWindowManager>();
        public static object GetInstance(Type type) => IoC.GetInstance(type, null);

        public static void ShowWindow<T>(this Screen screen, IDictionary<string, object> settings = null)
        {
            WindowManager.ShowWindow(GetInstance(typeof(T)), null, settings);
        }

        public static bool? ShowDialog<T>(this Screen screen, IDictionary<string, object> settings = null)
        {
            return WindowManager.ShowDialog(GetInstance(typeof(T)), null, settings);
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
