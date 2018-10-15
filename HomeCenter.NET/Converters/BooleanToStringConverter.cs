using System;
using System.Globalization;
using System.Windows.Data;

namespace HomeCenter.NET.Converters
{
    public class BooleanToStringConverter : IValueConverter
    {
        #region Properties

        public string TrueText { get; }
        public string FalseText { get; }

        #endregion

        #region Constructors

        protected BooleanToStringConverter(string trueText, string falseText)
        {
            TrueText = trueText;
            FalseText = falseText;
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
            var text = boolean ? TrueText : FalseText;

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
