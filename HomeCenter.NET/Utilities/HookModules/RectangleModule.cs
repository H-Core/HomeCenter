 using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using H.NET.Utilities;
using HomeCenter.NET.Views;
using Point = System.Drawing.Point;

namespace HomeCenter.NET.Utilities.HookModules
{
    public class RectangleModule : HookModule
    {
        public delegate void RectangleDelegate(Rectangle rectangle);
        public event RectangleDelegate NewRectangle;

        private RectangleView View { get; set; }
        private Point StartPoint { get; set; }
        private bool IsMouseDown { get; set; }

        public RectangleModule(List<Key> keys, List<ModifierKeys> modifiers) : base(keys, modifiers)
        {
        }

        public void Global_MouseUp(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = false;
            if (!IsHookCombination())
            {
                return;
            }

            e.Handled = true;

            View?.Close();
            View = null;

            var rectangle = CalculateRectangle(e);
            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            NewRectangle?.Invoke(rectangle);
        }

        public void Global_MouseMove(object sender, MouseEventExtArgs e)
        {
            if (!IsMouseDown || !IsHookCombination())
            {
                View?.Close();
                View = null;
                return;
            }

            var rectangle = CalculateRectangle(e);

            View.Border.Margin = new Thickness(
                rectangle.Left, 
                rectangle.Top, 
                View.Width - rectangle.Left - rectangle.Width, 
                View.Height - rectangle.Top - rectangle.Height);
            
        }

        public void Global_MouseDown(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = true;
            if (!IsHookCombination())
            {
                return;
            }

            e.Handled = true;

            View = new RectangleView();

            StartPoint = new Point(e.X, e.Y);

            View.Border.Margin = new Thickness(e.X, e.Y, View.Width - e.X, View.Height - e.Y);
            View.Border.Visibility = Visibility.Visible;
            
            View.Show();

        }

        private Rectangle CalculateRectangle(MouseEventExtArgs e)
        {
            var dx = e.X - StartPoint.X;
            var dy = e.Y - StartPoint.Y;

            var left = StartPoint.X + Math.Min(0, dx);
            var top = StartPoint.Y + Math.Min(0, dy);

            var width = dx > 0 ? dx : -dx;
            var height = dy > 0 ? dy : -dy;

            return new Rectangle(left, top, width, height);
        }
    }
}
