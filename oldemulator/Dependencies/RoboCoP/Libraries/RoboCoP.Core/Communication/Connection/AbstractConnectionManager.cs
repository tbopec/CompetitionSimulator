using System;
using System.Collections;
using System.Collections.Generic;
using RoboCoP.Helpers;
using RoboCoP.Internal;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Abstract implementation of <see cref="IConnectionsManager"/>.
    /// All <see cref="IConnectionsManager"/> are implemented but functionality of adding new <see cref="IConnection"/>s
    /// absence. It should be implemented by inheritors. Inheritor must inform <see cref="AbstractConnectionManager"/> about the fact
    /// that new connection was added using <see cref="AddConnection"/> method.
    /// </summary>
    public abstract class AbstractConnectionManager: IConnectionsManager
    {
        #region Implementation of IEnumerable

        /// <inheritdoc/>
        public IEnumerator<ISafeConnection> GetEnumerator()
        {
            if(disposed)
                throw new ObjectDisposedException("AbstractConnectionManager");

            return connections.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private readonly ICollection<ISafeConnection> connections =
            new ConcurentLinkedList<ISafeConnection>();

        private bool disposed;

        /// <summary>
        /// This method should be called by inheritors for adding new, just received <see cref="IConnection"/> to <see cref="AbstractConnectionManager"/>.
        /// </summary>
        protected void AddConnection(IConnection newConnection)
        {
            if(disposed)
                throw new ObjectDisposedException("AbstractConnectionManager");

            var safeConnection = new SafeConnection(newConnection);
            safeConnection.OnError += e => {
                                          connections.Remove(safeConnection);
                                          OnDisconnect(safeConnection);
                                      };
            connections.Add(safeConnection);
            OnConnect(safeConnection);
        }

        #region Implementation of IConnectionsManager

        /// <inheritdoc/>
        public event Action<ISafeConnection> OnDisconnect = delegate { };

        /// <inheritdoc/>
        public event Action<ISafeConnection> OnConnect = delegate { };

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if(disposed)
                return;
            disposed = true;
            foreach(ISafeConnection connection in connections)
                connection.Dispose();
            connections.Clear();
        }

        #endregion
    }
}