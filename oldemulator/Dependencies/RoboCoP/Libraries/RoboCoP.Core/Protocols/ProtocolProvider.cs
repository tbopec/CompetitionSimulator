using System;
using RoboCoP.Internal;

namespace RoboCoP.Protocols
{
    /// <summary>
    /// This class encapsulates creating of all protocol-specific essences.
    /// It's inheritors should not have generic type arguments
    /// and use concrete network address interface except <typeparamref name="T"/>.
    /// </summary>
    public abstract class ProtocolProvider<T>
        where T: INetworkAddress
    {
        /// <summary>
        /// Creates a delegate which creating <see cref="IConnection"/> with <paramref name="addressToConnectTo"/>.
        /// </summary>
        public abstract Func<IConnection> GetConnectionFunc(T addressToConnectTo);

        /// <summary>
        /// Creates a <see cref="IConnectionsManager"/> which is bound on <paramref name="addressToBindTo"/>.
        /// </summary>
        public abstract IConnectionsManager GetConnectionManager(T addressToBindTo);
    }
}