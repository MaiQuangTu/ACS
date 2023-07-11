using System;

namespace LIB.Infrastructure
{
    public class Disposable : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._isDisposed && disposing)
                DisposeCore();

            this._isDisposed = true;
        }

        // Ovveride this to dispose custom objects
        protected virtual void DisposeCore()
        {
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}