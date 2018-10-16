using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToGreenRedColorConverter : BooleanConverter
    {
        public BooleanToGreenRedColorConverter() : base(new SolidColorBrush(Colors.LightGreen), new SolidColorBrush(Colors.Red))
        {
        }
    }
}
