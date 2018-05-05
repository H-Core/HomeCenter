using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace HomeCenter.NET.Utilities
{
    public static class UiExtensions
    {
        public static void Update(this Panel panel, IEnumerable<Control> controls)
        {
            panel.Children.Clear();
            foreach (var control in controls)
            {
                panel.Children.Add(control);
            }
        }

        public static void AddButton(this IList<Control> list, string text, RoutedEventHandler handler)
        {
            var button = new Button
            {
                Content = text
            };
            button.Click += handler;

            list.Add(button);
        }

        public static bool IsModal(this Window window)
        {
            return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
        }

        public static void SetDialogResult(this Window window, bool? value)
        {
            try
            {
                if (!window.IsModal())
                {
                    return;
                }

                window.DialogResult = value;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void SetDialogResultAndClose(this Window window, bool? value)
        {
            window.SetDialogResult(value);
            window.Close();
        }
    }
}
