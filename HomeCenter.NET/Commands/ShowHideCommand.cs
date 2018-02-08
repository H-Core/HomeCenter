using System;
using System.Windows;
using System.Windows.Input;

namespace HomeCenter.NET.Commands
{
    public class ShowHideCommand : ICommand
    {
        public void Execute(object windowObject)
        {
            if (!(windowObject is Window window))
            {
                throw new ArgumentException(@"Window object is not window", nameof(windowObject));
            }

            if (window.Visibility == Visibility.Visible)
            {
                window.Hide();
            }
            else
            {
                window.Show();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
