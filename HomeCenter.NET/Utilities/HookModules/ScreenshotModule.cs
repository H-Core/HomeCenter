using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HomeCenter.NET.Extensions;

namespace HomeCenter.NET.Utilities.HookModules
{
    public class ScreenshotModule : RectangleModule
    {
        public ScreenshotModule() : base(new List<Key> { Key.Space }, new List<ModifierKeys>{ ModifierKeys.Shift })
        {
            NewImage += image => Clipboard.SetImage(image.ToBitmapImage());
        }
    }
}
