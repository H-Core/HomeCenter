using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToGreenBisqueConverter : BooleanConverter
    {
        public BooleanToGreenBisqueConverter() : base(new SolidColorBrush(Colors.LightGreen), new SolidColorBrush(Colors.Bisque))
        {
        }
    }
}
