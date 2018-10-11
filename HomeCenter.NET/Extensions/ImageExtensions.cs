using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace HomeCenter.NET.Extensions
{
    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this Image image)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = image.ToMemoryStream();
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static MemoryStream ToMemoryStream(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Bmp);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
