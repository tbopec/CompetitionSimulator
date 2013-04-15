using System;
using RoboCoP.Internal;
using RoboCoP.Messages;
using RoboCoP.Helpers;
using System.Linq;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Class for sending <see cref="Message"/>s of type <typeparamref name="TMessage"/> to underlying <see cref="ISafeConnection"/>s.
    /// Implementation of the <see cref="ISender{TMessage}"/>.
    /// </summary>
    public class SingleSender<TMessage>: ISender<TMessage>
        where TMessage: Message
    {
        protected readonly IStableConnection connection;
        private bool disposed;

        public SingleSender(IStableConnection connection)
        {
            this.connection = connection;
        }

        #region ISender<TMessage> Members

        /// <inheritdoc/>
        public IObservable<Unit> Send(TMessage message)
        {
            if(disposed)
                throw new ObjectDisposedException("SingleSender");
            if(message == null)
                throw new ArgumentNullException("message");

            return new MessageSerializer(message)
                .Package
                .SplitIntoPieces(connection.MaxSendLength)
                .Select(connection.Send)
                .Concat()
                .TakeLast(1);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if(disposed)
                return;
            disposed = true;
            connection.Dispose();
        }

        #endregion
    }
}