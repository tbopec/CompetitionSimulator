using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using RoboCoP;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{

    /// <summary>
    /// Implementation of <see cref="IIn"/>, which receive a binary data
    /// via <see cref="IReceiver{DataMessage}"/> and return his body as binary.
    /// </summary>
    public class DataIn : IIn
    {

        public readonly int ChannelIndex;

        private IReceiver<DataMessage> receiver;

        private bool disposed = false;

        public DataIn(int index, IReceiver<DataMessage> messageReceiver)
        {
            ChannelIndex = index;
            if (ReferenceEquals(messageReceiver, null))
                throw new ArgumentNullException("messageReceiver");

            receiver = messageReceiver;
        }

        /// <summary>
        /// Initialize an asynchronous receiving as an <see cref="IObservable{T}"/> 
        /// operation of a text data  ,that contains in body of one message.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        protected IObservable<string> ReceiveTextBase()
        {
            return receiver.Receive().Select(msg => msg.TextBody);
        }

        /// <summary>
        /// Initialize an asynchronous receiving as an <see cref="IObservable{T}"/>
        /// operation of a binary data, that contains in body of one message.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        protected IObservable<byte[]> ReceiveBinaryBase()
        {
            return receiver.Receive().Select(msg => msg.Body);
        }

        /// <summary>
        /// Directly synchronously receiving a text data.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public string ReceiveText()
        {
            return ReceiveTextBase().Single();
        }

        /// <summary>
        /// Directly starts async receiving a text data.
        /// Call the <paramref name="onReceived"/> when done.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IIn.ReceiveText"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void ReceiveTextAsync(Action<string> onReceived)
        {
            var receiveThread = new Thread(() => ReceiveTextBase().Subscribe(onReceived));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }


        /// <summary>
        /// Starts async receiving of all package.
        /// Call the <paramref name="onReceived"/> each time when a text data received.
        /// Call the <paramref name="onError"/> each time when error occurs.
        /// Returns the <see cref="IDisposable"/> calling <see cref="IDisposable.Dispose"/> on which will cancel receiving.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IIn.ReceiveText"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void ReceiveTextAll(Action<string> onReceived, Action<Exception> onError = null)
        {
            if (onError == null)
                onError = delegate { };
            var receiveThread = new Thread(() => ReceiveTextAllObservable().Subscribe(onReceived, onError));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        /// <summary>
        /// Directly synchronously receiving a binary data.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public byte[] ReceiveBinary()
        {
            return ReceiveBinaryBase().Single();
        }

        /// <summary>
        /// Directly starts async receiving a binary data.
        /// Call the <paramref name="onReceived"/> when done.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IIn.ReceiveText"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void ReceiveBinaryAsync(Action<byte[]> onReceived)
        {
            var receiveThread = new Thread(() => ReceiveBinaryBase().Subscribe(onReceived));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        /// <summary>
        /// Starts async receiving of all package.
        /// Call the <paramref name="onReceived"/> each time when a binary data received.
        /// Call the <paramref name="onError"/> each time when error occurs.
        /// Returns the <see cref="IDisposable"/> calling <see cref="IDisposable.Dispose"/> on which will cancel receiving.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IIn.ReceiveText"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void ReceiveBinaryAll(Action<byte[]> onReceived, Action<Exception> onError)
        {
            if (onError == null)
                onError = delegate { };
            var receiveThread = new Thread(() => ReceiveBinaryAllObservable().Subscribe(onReceived, onError));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private IObservable<string> ReceiveTextAllObservable()
        {
            return Observable.Defer(ReceiveTextBase).Repeat();
        }

        private IObservable<byte[]> ReceiveBinaryAllObservable()
        {
            return Observable.Defer(ReceiveBinaryBase).Repeat();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!disposed)
                return;
            disposed = true;
            receiver.Dispose();
        }
    }
}
