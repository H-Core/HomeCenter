using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace H.NET.Utilities
{
    // ReSharper disable once IdentifierTypo
    public static class Screenshoter
    {
        // P/Invoke declarations
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
            wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);

        public enum SystemMetric
        {
            VirtualScreenX = 76, // SM_XVIRTUALSCREEN
            VirtualScreenY = 77, // SM_YVIRTUALSCREEN
            VirtualScreenWidth = 78, // CXVIRTUALSCREEN 0x0000004E 
            VirtualScreenHeight = 79, // CYVIRTUALSCREEN 0x0000004F 
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric metric);

        public static Size GetVirtualDisplaySize()
        {
            var width = GetSystemMetrics(SystemMetric.VirtualScreenWidth);
            var height = GetSystemMetrics(SystemMetric.VirtualScreenHeight);

            return new Size(width, height);
        }

        public static Point GetVirtualDisplayStartPoint()
        {
            var x = GetSystemMetrics(SystemMetric.VirtualScreenX);
            var y = GetSystemMetrics(SystemMetric.VirtualScreenY);

            return new Point(x, y);
        }

        /// <summary>
        /// Required to Dispose after usage
        /// </summary>
        /// <returns></returns>
        public static Image ShotVirtualDisplay()
        {
            var size = GetVirtualDisplaySize();
            var startPoint = GetVirtualDisplayStartPoint();

            var window = GetDesktopWindow();
            var dc = GetWindowDC(window);
            var toDc = CreateCompatibleDC(dc);
            var hBmp = CreateCompatibleBitmap(dc, size.Width, size.Height);
            var hOldBmp = SelectObject(toDc, hBmp);

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            BitBlt(toDc, 0, 0, size.Width, size.Height, dc, startPoint.X, startPoint.Y, CopyPixelOperation.CaptureBlt | CopyPixelOperation.SourceCopy); //-V3059

            var image = Image.FromHbitmap(hBmp);
            SelectObject(toDc, hOldBmp);
            DeleteObject(hBmp);
            DeleteDC(toDc);
            ReleaseDC(window, dc);

            return image;
        }

        public static async Task<Image> ShotVirtualDisplayAsync() => await Task.Run(ShotVirtualDisplay);

        public static async Task<Image> ShotVirtualDisplayRectangleAsync(Rectangle rectangle)
        {
            using (var image = await ShotVirtualDisplayAsync())
            using (var bitmap = new Bitmap(image))
            {
                return bitmap.Clone(rectangle, bitmap.PixelFormat);
            }
        }
    }
}
