using System;
using System.Collections.Generic;
using System.Linq;
using RoboCoP.Exceptions;
using RoboCoP.Helpers;
using RoboCoP.Internal;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Class for receiving message of type <typeparamref name="TMessage"/> from the underlying <see cref="StableConnection"/>.
    /// Implementation of the <see cref="IReceiver{TMessage}"/>.
    /// </summary>
    public class Receiver<TMessage>: IReceiver<TMessage>
        where TMessage: Message
    {
        protected readonly IStableConnection stableConnection;
        private byte[] dataExcess = new byte[0];
        private bool disposed;

        public Receiver(IStableConnection stableConnection)
        {
            this.stableConnection = stableConnection;
        }

        #region Implementation of IDisposable

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if(disposed)
                return;
            disposed = true;
            stableConnection.Dispose();
        }

        #endregion

        #region IReceiver<TMessage> Members

        /// <inheritdoc/>
        public IObservable<TMessage> Receive()
        {
            if(disposed)
                throw new ObjectDisposedException("Receiver");

            MessageAccumulator accumulator = MessageAccumulator.Empty();

            bool firstCall = true;

            return Observable
                .Defer(() => {
                           if(firstCall) {
                               firstCall = false;
                               return Observable.Return(dataExcess);
                           }
                           return stableConnection.Receive();
                       })
                .Select(x => accumulator = AccumulateMessage(accumulator, x))
                .Where(ma => ma.MessageDeserializer.MessageFinished)
                .RepeatWhile(() => !accumulator.MessageDeserializer.MessageFinished)
                .Do(ma => dataExcess = ma.DataExcess.ToArray())
                .Select(ProduceMessage);
        }

        #endregion

        private static TMessage ProduceMessage(MessageAccumulator md)
        {
            Message msg = md.MessageDeserializer.ToMessage();
            if(!(msg is TMessage))
                throw new InvalidMessageTypeException(typeof(TMessage), msg.GetType());
            return (TMessage) msg;
        }

        private static MessageAccumulator AccumulateMessage(MessageAccumulator ma, IEnumerable<byte> newBytes)
        {
            MessageDeserializer md = ma.MessageDeserializer;

            IEnumerable<byte> bytes = ma.DataExcess.Concat(newBytes);
            IEnumerator<KeyValuePair<string, IEnumerable<byte>>> keyValuePairs = bytes.ToArray().CutLines().GetEnumerator();
            while(!md.HeaderFinished && keyValuePairs.MoveNext()) {
                md.AddHeaderLine(keyValuePairs.Current.Key);
                bytes = keyValuePairs.Current.Value;
            }

            if(md.HeaderFinished && bytes.Any() && md.RestBodyLength > 0) {
                IEnumerable<byte> data = bytes.Take(md.RestBodyLength);
                bytes = bytes.Skip(md.RestBodyLength);
                md.AddBytesToBody(data);
            }

            return new MessageAccumulator { MessageDeserializer = md, DataExcess = bytes };
        }

        #region Nested type: MessageAccumulator

        private struct MessageAccumulator
        {
            public IEnumerable<byte> DataExcess;
            public MessageDeserializer MessageDeserializer;

            public static MessageAccumulator Empty()
            {
                return new MessageAccumulator
                       {
                           MessageDeserializer = new MessageDeserializer(),
                           DataExcess = new byte[0]
                       };
            }
        }

        #endregion
    }
}