using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace H.NET.Notifiers.Extensions
{
    public static class BitmapExtensions
    {
        public static Mat ToMat(this Bitmap bitmap)
        {
            // Lock the bitmap's bits.  
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // scan0 is a pointer to our memory block.
            // stride = amount of bytes for a single line of the image
            // So you can try to get you Mat instance like this:
            var mat = new Mat(bitmap.Height, bitmap.Width, DepthType.Cv8U, 4, data.Scan0, data.Stride);

            // Unlock the bits.
            bitmap.UnlockBits(data);

            return mat;
        }
    }
}
