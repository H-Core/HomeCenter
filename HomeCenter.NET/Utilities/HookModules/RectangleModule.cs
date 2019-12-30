 using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using H.NET.Utilities;
using HomeCenter.NET.Views.Utilities;
using Point = System.Drawing.Point;

namespace HomeCenter.NET.Utilities.HookModules
{
    public class RectangleModule : HookModule
    {
        #region Properties

        private RectangleView? View { get; set; }
        private Point StartPoint { get; set; }
        private bool IsMouseDown { get; set; }

        #endregion

        #region Events

        public event EventHandler<Rectangle>? NewRectangle;

        private void OnNewRectangle(Rectangle rectangle)
        {
            NewRectangle?.Invoke(this, rectangle);
        }

        #endregion

        #region Constructors

        public RectangleModule(List<Key>? keys, List<ModifierKeys>? modifiers) : base(keys, modifiers)
        {
        }

        #endregion

        #region Event Handlers

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

            OnNewRectangle(rectangle);
        }

        public void Global_MouseMove(object sender, MouseEventExtArgs e)
        {
            //if (System.Diagnostics.Debugger.IsAttached)
            if (!IsMouseDown || !IsHookCombination())
            {
                View?.Close();
                View = null;
                return;
            }

            var rectangle = CalculateRectangle(e);

            View = View ?? throw new InvalidOperationException("View is null");
            View.Border.Margin = new Thickness(
                rectangle.Left - View.Left,
                rectangle.Top - View.Top,
                View.Width + View.Left - rectangle.Left - rectangle.Width,
                View.Height + View.Top - rectangle.Top - rectangle.Height);

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

            View.Border.Margin = new Thickness(
                e.X - View.Left,
                e.Y - View.Top,
                View.Width + View.Left - e.X,
                View.Height + View.Top - e.Y);
            View.Border.Visibility = Visibility.Visible;

            View.Show();

        }

        #endregion

        #region Private Methods

        private Rectangle CalculateRectangle(MouseEventExtArgs e)
        {
            var dx = e.X - StartPoint.X;
            var dy = e.Y - StartPoint.Y;

            var x = StartPoint.X + Math.Min(0, dx);
            var y = StartPoint.Y + Math.Min(0, dy);

            var width = dx > 0 ? dx : -dx;
            var height = dy > 0 ? dy : -dy;

            return new Rectangle(x, y, width, height);
        }

        #endregion
    }
}
