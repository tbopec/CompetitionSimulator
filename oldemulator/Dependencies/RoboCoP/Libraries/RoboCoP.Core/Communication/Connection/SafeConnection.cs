using System;
using System.IO;
using System.Linq;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Common implementation of <see cref="ISafeConnection"/>.
    /// </summary>
    public class SafeConnection: ISafeConnection
    {
        private readonly IConnection connection;
        private bool disposed;

        public SafeConnection(IConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");
            this.connection = connection;
            OnError += (_) => { };
            RemoteEndpoint = connection.RemoteEndpoint;
        }

        #region Implementation of ISafeConnection

        /// <inheritdoc/>
        public INetworkAddress RemoteEndpoint { get; private set; }

        /// <inheritdoc/>
        public IObservable<Unit> Send(byte[] data)
        {
            if(disposed)
                throw new ObjectDisposedException("SafeConnection");
            return connection
                .Send(data)
                .Catch<Unit, IOException>(ex => {
                                              OnError(ex);
                                              return Observable.Return(new Unit());
                                          });
        }

        /// <inheritdoc/>
        public IObservable<byte[]> Receive()
        {
            if (disposed)
                throw new ObjectDisposedException("SafeConnection");
            return connection
                .Receive()
                .Catch<byte[], IOException>(ex => {
                                                OnError(ex);
                                                return Observable.Return(new byte[0]);
                                            });
        }

        /// <inheritdoc/>
        public event Action<IOException> OnError;

        /// <inheritdoc/>
        public int MaxSendLength
        {
            get
            {
                if(disposed)
                    throw new ObjectDisposedException("SafeConnection");
                return connection.MaxSendLength;
            }
        }

        /// <inheritdoc/>
        public IConnection UnsafeConnection
        {
            get
            {
                if(disposed)
                    throw new ObjectDisposedException("SafeConnection");
                return connection;
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if(disposed)
                return;
            disposed = true;
            connection.Dispose();
        }

        #endregion
    }
}