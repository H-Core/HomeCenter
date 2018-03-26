using System;
using Emgu.CV;

namespace H.NET.Notifiers.Utilities
{
    public static class ScreenshotUtilities
    {
        private static double Epsilon { get; } = 0.05;

        public static double GetDifference(Mat mat1, Mat mat2, Mat mask = null)
        {
            if (mask != null && !mask.IsEmpty)
            {
                var mat1Masked = new Mat();
                mat1.CopyTo(mat1Masked, mask);
                mat1 = mat1Masked;

                var mat2Masked = new Mat();
                mat2.CopyTo(mat2Masked, mask);
                mat2 = mat2Masked;
            }

            var foreground = new Mat();
            CvInvoke.AbsDiff(mat1, mat2, foreground);

            return CvInvoke.Mean(foreground).V0;
        }

        public static bool IsEquals(double difference) => Math.Abs(difference) < Epsilon;
        public static bool IsEquals(Mat mat1, Mat mat2, Mat mask = null) => IsEquals(GetDifference(mat1, mat2, mask));
    }
}
