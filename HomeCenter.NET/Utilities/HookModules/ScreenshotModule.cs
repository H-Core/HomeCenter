using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using H.NET.Utilities;
using HomeCenter.NET.Extensions;

namespace HomeCenter.NET.Utilities.HookModules
{
    public class ScreenshotModule : RectangleModule
    {
        public ScreenshotModule() : base(new List<Key> { Key.LeftAlt, Key.RightAlt }, null)
        {
            NewRectangle += async rectangle =>
            {
                var startPoint = Screenshoter.GetVirtualDisplayStartPoint();
                rectangle.X -= startPoint.X;
                rectangle.Y -= startPoint.Y;

                using (var image = await Screenshoter.ShotVirtualDisplayRectangleAsync(rectangle))
                {
                    Clipboard.SetImage(image.ToBitmapImage());
                }
            };
        }
    }
}
