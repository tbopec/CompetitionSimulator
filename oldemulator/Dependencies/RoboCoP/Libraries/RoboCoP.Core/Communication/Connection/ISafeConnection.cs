using System;
using System.IO;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Protocol-independent interface for sending and receiving packages of raw data.
    /// It's methods never throws and only rises <see cref="OnError"/> in case of internal errors (for example disconnect).
    /// </summary>
    public interface ISafeConnection: IConnection, IDisposable
    {
        /// <summary>
        /// Max length of one piece of data which may be send via <see cref="Send"/>. Returns -1 if unlimited.
        /// </summary>
        int MaxSendLength { get; }

        /// <summary>
        /// Gets internal <see cref="IConnection"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">This property is not supported.</exception>
        IConnection UnsafeConnection { get; }

        /// <summary>
        /// This event is risen if some internal error appears during sending or receiving.
        /// </summary>
        event Action<IOException> OnError;
    }
}