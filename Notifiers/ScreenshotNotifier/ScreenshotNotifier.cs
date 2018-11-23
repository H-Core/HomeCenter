using System.Drawing;
using System.Threading.Tasks;
using H.NET.Utilities;

namespace H.NET.Notifiers
{
    public abstract class ScreenshotNotifier : TimerNotifier
    {
        #region Protected methods

        protected abstract bool Analyze(Bitmap bitmap);

        protected override async Task<bool> OnResultAsync()
        {
            using (var image = await Screenshoter.ShotVirtualDisplayAsync()) // TODO: shot main display ?
            using (var bitmap = new Bitmap(image))
            {
                return Analyze(bitmap);
            }
        }

        #endregion

    }
}
