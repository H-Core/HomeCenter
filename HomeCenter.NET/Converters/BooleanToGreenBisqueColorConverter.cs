using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToGreenBisqueColorConverter : BooleanConverter
    {
        public BooleanToGreenBisqueColorConverter() : base(new SolidColorBrush(Colors.LightGreen), new SolidColorBrush(Colors.Bisque))
        {
        }
    }
}
