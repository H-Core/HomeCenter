using System;

namespace H.NET.Core.Utilities
{
    public abstract class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
