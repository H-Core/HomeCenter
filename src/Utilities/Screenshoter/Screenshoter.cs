using System;
using System.ComponentModel;
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
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
            wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr ptr);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        public enum SystemMetric
        {
            VirtualScreenX = 76, // SM_XVIRTUALSCREEN
            VirtualScreenY = 77, // SM_YVIRTUALSCREEN
            VirtualScreenWidth = 78, // CXVIRTUALSCREEN 0x0000004E 
            VirtualScreenHeight = 79, // CYVIRTUALSCREEN 0x0000004F 
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric metric);

        public static Rectangle GetVirtualDisplayRectangle()
        {
            var x = (int)Math.Round(ConvertPixel(GetSystemMetrics(SystemMetric.VirtualScreenX)));
            var y = (int)Math.Round(ConvertPixel(GetSystemMetrics(SystemMetric.VirtualScreenY)));
            var width = (int)Math.Round(ConvertPixel(GetSystemMetrics(SystemMetric.VirtualScreenWidth)));
            var height = (int)Math.Round(ConvertPixel(GetSystemMetrics(SystemMetric.VirtualScreenHeight)));

            return new Rectangle(x, y, width, height);
        }

        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private static int? _dpi;
        private static readonly object _dpiLock = new object();

        internal static int Dpi {
            get {
                if (!_dpi.HasValue)
                {
                    lock (_dpiLock)
                    {
                        if (!_dpi.HasValue)
                        {
                            var dc = GetDC(IntPtr.Zero);
                            if (dc == IntPtr.Zero)
                            {
                                throw new Win32Exception();
                            }

                            try
                            {
                                _dpi = GetDeviceCaps(dc, 90);
                            }
                            finally
                            {
                                ReleaseDC(IntPtr.Zero, dc);
                            }
                        }
                    }
                }

                return _dpi.Value;
            }
        }

        public static double ConvertPixel(double pixel)
        {
            var dpi = Dpi;

            return dpi != 0 
                ? pixel * 96.0 / dpi 
                : pixel;
        }

        /// <summary>
        /// Required to Dispose after usage
        /// </summary>
        /// <returns></returns>
        public static (Rectangle rectangle, Image image) ShotVirtualDisplay()
        {
            var rectangle = GetVirtualDisplayRectangle();

            var window = GetDesktopWindow();
            var dc = GetWindowDC(window);
            var toDc = CreateCompatibleDC(dc);
            var hBmp = CreateCompatibleBitmap(dc, rectangle.Width, rectangle.Height);
            var hOldBmp = SelectObject(toDc, hBmp);

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            BitBlt(toDc, 0, 0, rectangle.Width, rectangle.Height, dc, rectangle.X, rectangle.Y, CopyPixelOperation.CaptureBlt | CopyPixelOperation.SourceCopy); //-V3059

            var bitmap = Image.FromHbitmap(hBmp);
            SelectObject(toDc, hOldBmp);
            DeleteObject(hBmp);
            DeleteDC(toDc);
            ReleaseDC(window, dc);

            return (rectangle, bitmap);
        }

        public static async Task<Image> ShotVirtualDisplayAsync() => (await Task.Run(ShotVirtualDisplay)).image;

        public static async Task<Image> ShotVirtualDisplayRectangleAsync(Rectangle rectangle)
        {
            var (displayRectangle, image) = await Task.Run(ShotVirtualDisplay);

            rectangle.X -= displayRectangle.X;
            rectangle.Y -= displayRectangle.Y;

            using (image)
            using (var bitmap = new Bitmap(image))
            {
                return bitmap.Clone(rectangle, bitmap.PixelFormat);
            }
        }
    }
}
