using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToGreenBisqueColorConverter : BooleanToColorConverter
    {
        public BooleanToGreenBisqueColorConverter() : base(Colors.LightGreen, Colors.Bisque)
        {
        }
    }
}
