namespace HomeCenter.NET.Converters
{
    public class BooleanToOnOffConverter : BooleanToStringConverter
    {
        public BooleanToOnOffConverter() : base("On", "Off")
        {
        }
    }
}
