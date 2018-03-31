using System;

namespace H.NET.Plugins
{
    public class RuntimeObject<T> : IDisposable where T : class
    {
        #region Fields

        private T _value;
        private Exception _exception;

        #endregion

        #region Properties

        public T Value
        {
            get => _value;
            set {
                Dispose();

                Exception = null;
                _value = value;
            }
        }

        public Exception Exception
        {
            get => _exception;
            set {
                Dispose();

                _exception = value;
            }
        }

        public bool IsEnabled => Value != null;

        #endregion

        #region Constructors

        public RuntimeObject()
        {
        }

        public RuntimeObject(T value)
        {
            Value = value;
        }

        public RuntimeObject(Exception exception)
        {
            Exception = exception;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _value = null;
        }

        #endregion

    }
}