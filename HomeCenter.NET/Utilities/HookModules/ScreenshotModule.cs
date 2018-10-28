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
                using (var image = await Screenshoter.ShotRectangleAsync(rectangle))
                {
                    Clipboard.SetImage(image.ToBitmapImage());
                }
            };
        }
    }
}
