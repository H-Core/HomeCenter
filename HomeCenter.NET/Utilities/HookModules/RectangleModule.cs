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
        public delegate void ImageDelegate(Image image);
        public event ImageDelegate NewImage;

        private RectangleView RectangleView { get; set; }
        private Point RectanglePoint { get; set; }
        private bool IsMouseDown { get; set; }

        public RectangleModule(List<Key> keys, List<ModifierKeys> modifiers) : base(keys, modifiers)
        {
        }

        public async void Global_MouseUp(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = false;
            if (!IsHookCombination())
            {
                return;
            }

            e.Handled = true;

            RectangleView.Close();
            RectangleView = null;

            var rectangle = CalculateRectangle(e);
            if (rectangle.Width == 0 || rectangle.Height == 0)
            {
                return;
            }

            using (var image = await Screenshoter.ShotRectangleAsync(rectangle))
            {
                NewImage?.Invoke(image);
            }
        }

        public void Global_MouseMove(object sender, MouseEventExtArgs e)
        {
            if (!IsMouseDown || !IsHookCombination())
            {
                RectangleView.Close();
                RectangleView = null;
                return;
            }

            //e.Handled = true;

            var rectangle = CalculateRectangle(e);

            RectangleView.Left = rectangle.Left;
            RectangleView.Top = rectangle.Top;

            RectangleView.Width = rectangle.Width;
            RectangleView.Height = rectangle.Height;
        }

        public void Global_MouseDown(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = true;
            if (!IsHookCombination())
            {
                return;
            }

            e.Handled = true;

            RectangleView = new RectangleView();
            RectangleView.Show();
            RectangleView.Left = e.X;
            RectangleView.Top = e.Y;
            RectangleView.Width = 0;
            RectangleView.Height = 0;

            RectanglePoint = new Point(e.X, e.Y);
        }

        private Rectangle CalculateRectangle(MouseEventExtArgs e)
        {
            var dx = e.X - RectanglePoint.X;
            var dy = e.Y - RectanglePoint.Y;

            var left = RectanglePoint.X + Math.Min(0, dx);
            var top = RectanglePoint.Y + Math.Min(0, dy);

            var width = dx > 0 ? dx : -dx;
            var height = dy > 0 ? dy : -dy;

            return new Rectangle(left, top, width, height);
        }
    }
}
