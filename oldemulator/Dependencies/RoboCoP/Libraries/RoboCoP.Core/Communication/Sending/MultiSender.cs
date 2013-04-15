using System;
using System.Linq;
using RoboCoP.Helpers;
using RoboCoP.Internal;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Class for sending similar <see cref="Message"/>s of type <typeparamref name="TMessage"/> to multiple underlying <see cref="IConnection"/>s.
    /// Implementation of the <see cref="ISender{TMessage}"/>.
    /// </summary>
    public class MultiSender<TMessage>: ISender<TMessage>
        where TMessage: Message
    {
        protected readonly IConnectionsManager connectionsManager;
        private bool disposed;

        public MultiSender(IConnectionsManager connectionsManager)
        {
            if(connectionsManager == null)
                throw new ArgumentNullException();
            this.connectionsManager = connectionsManager;
        }

        #region ISender<TMessage> Members

        /// <inheritdoc/>
        public IObservable<Unit> Send(TMessage message)
        {
            if(disposed)
                throw new ObjectDisposedException("MultiSender");
            if(message == null)
                throw new ArgumentNullException("message");

            byte[] data = new MessageSerializer(message).Package;
            return
                connectionsManager
                    .SelectMany(x =>
                                    data
                                    .SplitIntoPieces(x.MaxSendLength)
                                    .Select(x.Send)
                    )
                    .DefaultIfEmpty(Observable.Return(new Unit()))
                    .Concat()
                    .TakeLast(1);
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if(disposed)
                return;
            disposed = true;
            connectionsManager.Dispose();
        }

        #endregion
    }
}