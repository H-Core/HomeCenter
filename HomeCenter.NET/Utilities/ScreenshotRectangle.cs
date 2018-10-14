using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using H.NET.Utilities;
using HomeCenter.NET.Views.Commands;
using Point = System.Drawing.Point;

namespace HomeCenter.NET.Utilities
{
    public static class ScreenshotRectangle
    {
        public static List<Key> ActivationKeys { get; set; } = new List<Key>();
        public static List<ModifierKeys> ActivationModifiers { get; set; } = new List<ModifierKeys>();

        public delegate void ImageDelegate(Image image);
        public static event ImageDelegate NewImage;

        private static RectangleView RectangleView { get; } = new  RectangleView();
        private static Point RectanglePoint { get; set; }
        private static bool IsMouseDown { get; set; }

        private static bool IsScreenshotCombination() =>
            ActivationKeys.All(Keyboard.IsKeyDown) && ActivationModifiers.All(i => (Keyboard.Modifiers & i) == i);

        public static async void Global_MouseUp(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = false;
            if (!IsScreenshotCombination())
            {
                return;
            }

            RectangleView.Hide();

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

        public static void Global_MouseMove(object sender, MouseEventExtArgs e)
        {
            if (!IsMouseDown || !IsScreenshotCombination())
            {
                RectangleView.Hide();
                return;
            }

            var rectangle = CalculateRectangle(e);

            RectangleView.Left = rectangle.Left;
            RectangleView.Top = rectangle.Top;

            RectangleView.Width = rectangle.Width;
            RectangleView.Height = rectangle.Height;
        }

        public static void Global_MouseDown(object sender, MouseEventExtArgs e)
        {
            IsMouseDown = true;
            if (!IsScreenshotCombination())
            {
                return;
            }

            RectangleView.Show();
            RectangleView.Left = e.X;
            RectangleView.Top = e.Y;
            RectangleView.Width = 0;
            RectangleView.Height = 0;

            RectanglePoint = new Point(e.X, e.Y);
        }

        private static Rectangle CalculateRectangle(MouseEventExtArgs e)
        {
            var dx = e.X - RectanglePoint.X;
            var dy = e.Y - RectanglePoint.Y;

            var left = RectanglePoint.X + Math.Min(0, dx);
            var top = RectanglePoint.Y + Math.Min(0, dy);

            var width = dx > 0 ? dx : -dx;
            var height = dy > 0 ? dy : -dy;

            return new Rectangle((int)left, (int)top, (int)width, (int)height);
        }
    }
}
