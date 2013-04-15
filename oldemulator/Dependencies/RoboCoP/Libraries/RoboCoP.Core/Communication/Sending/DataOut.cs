using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RoboCoP;
using RoboCoP.Internal;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{

    /// <summary>
    /// Implementation of <see cref="IOut"/>, which converts a binary data in <see cref="DataMessage"/>
    /// and send it via <see cref="ISender{TMessage}"/>
    /// </summary>
    public class DataOut : IDataOut
    {

        private DataMessageFactory factory;

        public readonly int ChannelIndex;

        private ISender<DataMessage> sender;

        private bool disposed = false;

        public DataOut(int index, ISender<DataMessage> messageSender, DataMessageFactory messageBuider)
        {
            ChannelIndex = index;
            factory = messageBuider;
            sender = messageSender;
        }

        /// <summary>
        /// Initialize an asynchronous sending of the <paramref name="data"/> as an <see cref="IObservable{T}"/> operation.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// Provide a push-based notification of completion or error via that <see cref="IObservable{T}"/>.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        public IObservable<Unit> SendBase(string data, IDictionary<string, string> additionalHeaders = null)
        {
            return sender.Send(factory.DataMessage(data, additionalHeaders));
        }

        /// <summary>
        /// Initialize an asynchronous sending of the <paramref name="data"/> as an <see cref="IObservable{T}"/> operation.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// Provide a push-based notification of completion or error via that <see cref="IObservable{T}"/>.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        public IObservable<Unit> SendBase(byte[] data, IDictionary<string, string> additionalHeaders = null)
        {
            return sender.Send(factory.DataMessage(data, additionalHeaders));
        }

        
        /// <summary>
        /// Directly synchronously sends the <paramref name="data"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="SendBase"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void SendText(string data)
        {
            SendBase(data).Single();
        }

        /// <summary>
        /// Directly start async sending the <paramref name="data"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="SendBase"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void SendTextAsync(string data)
        {
            SendBase(data).Subscribe();
        }

        /// <summary>
        /// Directly start async sending the <paramref name="data"/>.
        /// Call <paramref name="onSent"/> when done.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="SendBase"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void SendTextAsync(string data, Action onSent)
        {
            SendBase(data).Subscribe(_ => onSent());
        }

        /// <summary>
        /// Directly synchronously sends the <paramref name="data"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="SendBase"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void SendBinary(byte[] data)
        {
            SendBase(data).Single();
        }

        /// <summary>
        /// Directly start async sending the <paramref name="data"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="SendBase"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void SendBinaryAsync(byte[] data)
        {
            SendBase(data).Subscribe();
        }

        /// <summary>
        /// Directly start async sending the <paramref name="data"/>.
        /// Call <paramref name="onSent"/> when done.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="SendBase"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public void SendBinaryAsync(byte[] data, Action onSent)
        {
            SendBase(data).Subscribe(_ => onSent());
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!disposed)
                return;
            disposed = true;
            sender.Dispose();
        }

    }
}
    