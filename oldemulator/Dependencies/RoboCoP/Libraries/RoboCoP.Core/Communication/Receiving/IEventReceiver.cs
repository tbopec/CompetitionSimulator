using System;
using System.IO;
using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Event-driven interface for receiving <typeparamref name="TMessage"/>s of type <typeparamref name="TMessage"/>.
    /// Stop receiving when <see cref="IDisposable.Dispose"/> called.
    /// </summary>
    public interface IEventReceiver<out TMessage>: IDisposable
        where TMessage: Message
    {
        /// <summary>
        /// Occurs when new <see cref="Message"/>s of type <typeparamref name="TMessage"/> received.
        /// </summary>
        event Action<TMessage> OnReceive;

        /// <summary>
        /// Occurs in case of an exception when receiving a <see cref="Message"/>.
        /// </summary>
        event Action<IOException> OnError;
    }
}