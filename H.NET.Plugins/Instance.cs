using System;
using System.Linq;

namespace H.NET.Plugins
{
    public class Instance<T> : IDisposable where T : class
    {
        #region Static methods

        public static Instance<T> FromString(string text)
        {
            var values = text.Split(Separator);

            return new Instance<T>
            {
                Name = values.ElementAtOrDefault(0),
                TypeName = values.ElementAtOrDefault(1),
                IsEnabled = bool.TryParse(values.ElementAtOrDefault(2), out var result) && result
            };
        }

        #endregion

        #region Properties

        public const char Separator = ' ';

        public string Name { get; set; }
        public string TypeName { get; set; }

        public bool IsEnabled { get; set; }
        public T Value { get; private set; }
        public Exception Exception { get; private set; }

        #endregion

        #region Public methods

        public override string ToString() => $"{Name}{Separator}{TypeName}{Separator}{IsEnabled}";

        public void SetException(Exception exception)
        {
            Dispose();

            Exception = exception;
        }

        public void SetValue(T value)
        {
            Dispose();

            Exception = null;
            Value = value;
            IsEnabled = true;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (Value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Value = null;
            IsEnabled = false;
        }

        #endregion

    }
}