using System;
using System.IO;

namespace RoboCoP.Internal
{
    public static class ConnectionProxies
    {
        /// <summary>
        /// Cast <paramref name="connection"/> to <see cref="ISafeConnection"/> without adding any 'safe' functionality.
        /// </summary>
        public static ISafeConnection AsSafe(this IConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");
            return new FakeStableSafeConnection(connection);
        }

        /// <summary>
        /// Cast <paramref name="connection"/> to <see cref="IStableConnection"/> without adding any 'stable' functionality.
        /// </summary>
        public static IStableConnection AsStable(this IConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");
            return new FakeStableSafeConnection(connection);
        }

        /// <summary>
        /// Cast <paramref name="connection"/> to <see cref="IStableConnection"/> without adding any 'stable' functionality.
        /// </summary>
        public static IStableConnection AsStable(this ISafeConnection connection)
        {
            if (ReferenceEquals(connection, null))
                throw new ArgumentNullException("connection");
            return new FakeStableConnection(connection);
        }

        #region Nested type: FakeStableSafeConnection

        private class FakeStableSafeConnection: ISafeConnection, IStableConnection, IConnection
        {
            private readonly IConnection connection;

            public FakeStableSafeConnection(IConnection connection)
            {
                this.connection = connection;
            }

            #region IConnection Members

            public INetworkAddress RemoteEndpoint
            {
                get { return connection.RemoteEndpoint; }
            }

            #endregion

            #region ISafeConnection Members

            public void Dispose()
            {
                connection.Dispose();
            }

            public int MaxSendLength
            {
                get { return connection.MaxSendLength; }
            }

            public IConnection UnsafeConnection
            {
                get { return this; }
            }

            public IObservable<byte[]> Receive()
            {
                return connection.Receive();
            }

            public IObservable<Unit> Send(byte[] data)
            {
                return connection.Send(data);
            }

            public event Action<IOException> OnError;

            #endregion
        }

        #endregion

        private class FakeStableConnection : IStableConnection
        {
            private readonly ISafeConnection safeConnection;

            public FakeStableConnection(ISafeConnection safeConnection)
            {
                this.safeConnection = safeConnection;
            }


            #region IStableConnection Members

            public void Dispose()
            {
                safeConnection.Dispose();
            }

            public int MaxSendLength
            {
                get { return safeConnection.MaxSendLength; }
            }

            public IConnection UnsafeConnection
            {
                get { return safeConnection.UnsafeConnection; }
            }

            public IObservable<byte[]> Receive()
            {
                return safeConnection.Receive();
            }

            public IObservable<Unit> Send(byte[] data)
            {
                return safeConnection.Send(data);
            }

            #endregion
        }


    }
}