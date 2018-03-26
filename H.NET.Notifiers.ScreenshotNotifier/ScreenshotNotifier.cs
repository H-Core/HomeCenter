using System.Drawing;

namespace H.NET.Notifiers
{
    public abstract class ScreenshotNotifier : TimerNotifier
    {
        #region Protected methods

        protected abstract bool Analyze(Bitmap bitmap);

        protected override async void OnElapsed()
        {
            using (var image = await Screenshoter.ShotAsync())
            using (var bitmap = new Bitmap(image))
            {
                if (Analyze(bitmap))
                {
                    OnEvent();
                }
            }
        }

        #endregion

    }
}
