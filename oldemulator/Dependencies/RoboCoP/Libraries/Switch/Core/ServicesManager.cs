using System;
using System.Collections.Generic;
using RoboCoP;
using RoboCoP.Helpers;
using RoboCoP.Implementation;
using RoboCoP.Internal;

namespace Switch.Core
{
    public abstract class ServicesManager: IDisposable
    {
        private readonly ConcurentLinkedList<ISafeConnection> connections = new ConcurentLinkedList<ISafeConnection>();
        private readonly IConnectionsManager connectionsManager;
        private bool disposed;

        protected ServicesManager(IConnectionsManager connectionsManager)
        {
            if(connectionsManager == null)
                throw new ArgumentNullException("connectionsManager");
            this.connectionsManager = connectionsManager;
            connectionsManager.OnConnect += AddConnection;
            connectionsManager.OnDisconnect += connection => RemoveConnection(connection, "OnDisconnect event");
        }

        #region Managing connections

        private void RemoveConnection(ISafeConnection safeConnection, string message)
        {
            if(!connections.Remove(safeConnection))
                return;
            ConnectionRemoved(message, safeConnection);
            safeConnection.Dispose();
        }

        private void AddConnection(ISafeConnection safeConnection)
        {
            connections.Add(safeConnection);
            ConnectionAdded(safeConnection);

            new Receiver<AddressableMessage>(CreateStableConnection(safeConnection))
                .ReceiveAll(message => ReceiveAddressableMessage(message, safeConnection),
                            exception => RemoveConnection(safeConnection, exception.Message));
        }

        private StableConnection CreateStableConnection(ISafeConnection safeConnection)
        {
            bool alreadyCreated = false;
            return new StableConnection(() => {
                                            if(alreadyCreated)
                                                RemoveConnection(safeConnection, "StableConnection failed");
                                            alreadyCreated = true;
                                            return safeConnection.UnsafeConnection;
                                        });
        }

        #endregion

        #region Protected interface

        protected IEnumerable<ISafeConnection> Connections
        {
            get { return connections; }
        }

        protected void DisconnectConnection(ISafeConnection safeConnection)
        {
            BeforeDisconnected(safeConnection);
            RemoveConnection(safeConnection, "Connection disconnected");
        }

        protected abstract void ReceiveAddressableMessage(AddressableMessage addressableMessage, ISafeConnection safeConnection);

        #endregion

        #region Disposing

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
                return;
            disposed = true;
            if(disposing) {
                connections.ForEach(DisconnectConnection);
                connectionsManager.Dispose();
            }
            connections.Clear();
        }

        ~ServicesManager()
        {
            Dispose(false);
        }

        #endregion

        #region Events

        public event Action<ISafeConnection> BeforeDisconnected = delegate { };
        public event Action<ISafeConnection> ConnectionAdded = delegate { };
        public event Action<string, ISafeConnection> ConnectionRemoved = delegate { };

        #endregion
    }
}