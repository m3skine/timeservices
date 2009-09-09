namespace Instinct_.Pattern
{
    /// <summary>
    /// Abstract class used as a base for all disposable singleton types in Instinct.
    /// </summary>
    public class Disposeable : System.IDisposable
    {
        private bool _isDisposed = false;

        /// <summary>
        /// Implements the <see cref="System.IDisposable.Dispose"/> method of
        /// <see cref="System.IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed == false)
            {
                _isDisposed = true;
                Dispose(true);
            }
        }

        /// <summary>
        /// Virtual method provided for derived classes to implement.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> release any unmanaged resources used by class.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}