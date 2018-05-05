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
    }
}
