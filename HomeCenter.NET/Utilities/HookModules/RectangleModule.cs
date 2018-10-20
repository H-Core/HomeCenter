using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using H.NET.Utilities;
using HomeCenter.NET.Views.Commands;
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

            //e.Handled = true;

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
                View.Close();
                View = null;
                return;
            }

            //e.Handled = true;

            // TODO: to full screen view, change only rectangle
            var rectangle = CalculateRectangle(e);

            View.Left = rectangle.Left;
            View.Top = rectangle.Top;

            View.Width = rectangle.Width;
            View.Height = rectangle.Height;
        }

        public void Global_MouseDown(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = true;
            if (!IsHookCombination())
            {
                return;
            }

            //e.Handled = true;

            View = new RectangleView();
            View.Show();
            View.Left = e.X;
            View.Top = e.Y;
            View.Width = 0;
            View.Height = 0;

            StartPoint = new Point(e.X, e.Y);
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
