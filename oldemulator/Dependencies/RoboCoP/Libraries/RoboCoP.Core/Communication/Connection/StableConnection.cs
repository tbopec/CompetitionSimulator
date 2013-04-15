using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Implementation of <see cref="IStableConnection"/>.
    /// Thread-unsafe.
    /// </summary>
    public class StableConnection: IStableConnection
    {
        private const int reconectionDelay = 100;
        private readonly Func<IConnection> connectionBuilder;
        private IConnection currentUnsafeConnection;

        private bool disposed;

        private bool IsConnect = false;

        private Object ReleaseConnectBlock = new Object();

        /// <summary>
        /// Construct <see cref="StableConnection"/> and get first value of <see cref="UnsafeConnection"/>
        /// (which leads to invoking <paramref name="connectionBuilder"/>).
        /// </summary>
        public StableConnection(Func<IConnection> connectionBuilder)
        {
            if(connectionBuilder == null)
                throw new ArgumentNullException();
            this.connectionBuilder = connectionBuilder;
            new Thread(ReleaseConnection).Start(); 
        }

        protected IConnection CurrentUnsafeConnection
        {
            get
            {
                if(currentUnsafeConnection == null)
                    ReleaseConnection();
                return currentUnsafeConnection;
            }
        }

        #region IStableConnection Members

        /// <inheridoc/>
        public IObservable<Unit> Send(byte[] data)
        {
            if(disposed)
                throw new ObjectDisposedException("StableConnection");
            return CurrentUnsafeConnection
                .Send(data)
                .Catch<Unit, IOException>(_ => {
                                              ReleaseConnection();
                                              return Send(data);
                                          });
        }

        /// <inheridoc/>
        public IObservable<byte[]> Receive()
        {
            if (disposed)
                throw new ObjectDisposedException("StableConnection");
            return CurrentUnsafeConnection
                .Receive()
                .Select(data =>
                        {
                            if (data.Length == 0)
                                ReleaseConnection();
                            return data;
                        })
                .Where(data => data.Length != 0)
                .Catch<byte[], IOException>(_ =>
                                            {
                                                ReleaseConnection();
                                                return Receive();
                                            });
        }

        /// <inheridoc/>
        public int MaxSendLength
        {
            get
            {
                if(disposed)
                    throw new ObjectDisposedException("StableConnection");
                try {
                    return CurrentUnsafeConnection.MaxSendLength;
                }
                catch(IOException) {
                    ReleaseConnection();
                    return MaxSendLength;
                }
            }
        }

        /// <inheridoc/>
        public IConnection UnsafeConnection
        {
            get
            {
                if(disposed)
                    throw new ObjectDisposedException("StableConnection");
                return CurrentUnsafeConnection;
            }
        }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if(disposed)
                return;
            disposed = true;
            if(currentUnsafeConnection != null)
                currentUnsafeConnection.Dispose();
        }

        #endregion

        protected void ReleaseConnection()
        {
            IsConnect = false;
            lock (ReleaseConnectBlock)
            {
                if (IsConnect)
                {
                    return;
                }
                if (currentUnsafeConnection != null)
                    currentUnsafeConnection.Dispose();
                currentUnsafeConnection = null;
                while (!IsConnect)
                {
                    try
                    {
                        currentUnsafeConnection = connectionBuilder();
                        IsConnect = true;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(reconectionDelay);
                    }
                }
            }
        }
    
    }
}