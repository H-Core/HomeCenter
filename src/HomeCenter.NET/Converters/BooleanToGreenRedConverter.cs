using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToGreenRedConverter : BooleanConverter
    {
        public BooleanToGreenRedConverter() : base(new SolidColorBrush(Colors.LightGreen), new SolidColorBrush(Colors.Red))
        {
        }
    }
}
