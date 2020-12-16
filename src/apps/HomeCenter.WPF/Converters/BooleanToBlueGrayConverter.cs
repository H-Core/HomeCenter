using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToBlueGrayConverter : BooleanConverter
    {
        public BooleanToBlueGrayConverter() : base(Brushes.LightSkyBlue, Brushes.LightGray)
        {
        }
    }
}
