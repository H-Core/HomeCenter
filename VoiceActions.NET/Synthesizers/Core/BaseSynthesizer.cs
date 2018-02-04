using System;

namespace VoiceActions.NET.Synthesizers.Core
{
    public class BaseSynthesizer : IDisposable
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
