using System;

namespace VoiceActions.NET.Converters.Core
{
    public class BaseConverter : IDisposable
    {
        #region Properties

        public Exception Exception { get; protected set; }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
