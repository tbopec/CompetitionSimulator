using System;
using System.IO;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Class for receiving message of type <typeparamref name="TMessage"/> via event-driven interface.
    /// Wrapper around push-based interface.
    /// Implementation of the <see cref="IEventReceiver{TMessage}"/>.
    /// </summary>
    public class EventReceiver<TMessage>: IEventReceiver<TMessage>
        where TMessage: Message
    {
        private readonly IDisposable disposable;

        public EventReceiver(IObservable<TMessage> observable)
        {
            disposable = observable.Subscribe(OnReceive, exception => OnError((IOException) exception));
        }

        #region IEventReceiver<TMessage> Members

        /// <inheritdoc />
        public event Action<TMessage> OnReceive = delegate { };

        /// <inheritdoc />
        public event Action<IOException> OnError = delegate { };

        /// <inheritdoc />
        public virtual void Dispose()
        {
            disposable.Dispose();
        }

        #endregion
    }
}