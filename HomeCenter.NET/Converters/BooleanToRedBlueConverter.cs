using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToRedBlueConverter : BooleanConverter
    {
        public BooleanToRedBlueConverter() : base(Brushes.OrangeRed, Brushes.RoyalBlue)
        {
        }
    }
}
