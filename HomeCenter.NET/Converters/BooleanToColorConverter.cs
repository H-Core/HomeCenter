using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HomeCenter.NET.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        #region Properties

        public Color TrueColor { get; }
        public Color FalseColor { get; }

        #endregion

        #region Constructors

        protected BooleanToColorConverter(Color trueColor, Color falseColor)
        {
            TrueColor = trueColor;
            FalseColor = falseColor;
        }

        #endregion

        #region Public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var boolean = (bool)value;
            var color = boolean ? TrueColor : FalseColor;

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
