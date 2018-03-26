using System.Drawing;
using System.IO;
using Emgu.CV;
using H.NET.Core.Settings;
using H.NET.Notifiers.Extensions;
using H.NET.Notifiers.Utilities;

namespace H.NET.Notifiers
{
    public class MaskScreenshotNotifier : ScreenshotNotifier
    {
        #region Properties

        private string _maskPath;
        public string MaskPath
        {
            get => _maskPath;
            set
            {
                _maskPath = value;

                if (!PathIsValid(value))
                {
                    return;
                }

                Mask = new Mat(value).ToGray();
            }
        }

        private Mat Mask { get; set; }

        #endregion

        #region Contructors

        public MaskScreenshotNotifier()
        {
            AddSetting("ExamplePath", o => MaskPath = o, PathIsValid, string.Empty, SettingType.Path);
        }

        #endregion

        #region Protected methods

        public static bool PathIsValid(string path) => !string.IsNullOrWhiteSpace(path) && File.Exists(path);

        protected override bool Analyze(Bitmap bitmap)
        {
            using (var mat = bitmap.ToMat())
            using (var grayMat = mat.ToGray())
            {
                return ScreenshotUtilities.IsEquals(grayMat, Mask, Mask);
            }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Mask?.Dispose();
            Mask = null;
        }

        #endregion
    }
}