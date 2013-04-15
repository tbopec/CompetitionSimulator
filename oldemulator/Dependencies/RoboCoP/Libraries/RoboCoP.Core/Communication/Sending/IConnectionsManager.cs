using System;
using System.Collections.Generic;
using RoboCoP.Internal;

namespace RoboCoP
{
    /// <summary>
    /// This interface manages all <see cref="IConnection"/>s which are associated with one outgoing Robocop connection.
    /// It encapsulates protocol-specific functionality of adding new <see cref="IConnection"/> and removing broken.
    /// </summary>
    public interface IConnectionsManager: IEnumerable<ISafeConnection>, IDisposable
    {
        /// <summary>
        /// <see cref="ISafeConnection"/> has been removed.
        /// </summary>
        event Action<ISafeConnection> OnDisconnect;

        /// <summary>
        /// New <see cref="ISafeConnection"/> were created.
        /// </summary>
        event Action<ISafeConnection> OnConnect;
    }
}