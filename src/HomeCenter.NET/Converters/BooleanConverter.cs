using System;
using System.Globalization;
using System.Windows.Data;

namespace HomeCenter.NET.Converters
{
    public class BooleanConverter : IValueConverter
    {
        #region Properties

        public object TrueObject { get; }
        public object FalseObject { get; }

        #endregion

        #region Constructors

        protected BooleanConverter(object trueObject, object falseObject)
        {
            TrueObject = trueObject;
            FalseObject = falseObject;
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
            var obj = boolean ? TrueObject : FalseObject;

            return obj;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, TrueObject);
        }

        #endregion
    }
}
