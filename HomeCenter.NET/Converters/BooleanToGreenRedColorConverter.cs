using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToGreenRedColorConverter : BooleanToColorConverter
    {
        public BooleanToGreenRedColorConverter() : base(Colors.LightGreen, Colors.Red)
        {
        }
    }
}
