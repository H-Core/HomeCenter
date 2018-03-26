using Emgu.CV;
using Emgu.CV.CvEnum;

namespace H.NET.Notifiers.Extensions
{
    public static class MatExtensions
    {
        public static Mat ToGray(this Mat mat)
        {
            var grayMat = new Mat();
            CvInvoke.CvtColor(mat, grayMat, ColorConversion.Bgra2Gray);

            mat.Dispose();
            
            return grayMat;
        }
    }
}
