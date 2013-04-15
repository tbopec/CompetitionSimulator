using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Interface for receiving a data from RoboCoP's network.
    /// </summary>
    public interface IIn : IDisposable
    {

        /// <summary>
        /// Receiving a text data.
        /// </summary>
        string ReceiveText();

        /// <summary>
        /// Starts async receiving a text data.
        /// Call the <paramref name="onReceived"/> when done.
        /// </summary>
        void ReceiveTextAsync(Action<string> onReceived);

        /// <summary>
        /// Starts async receiving of all strings.
        /// Call the <paramref name="onReceived"/> each time when package received.
        /// Call the <paramref name="onError"/> each time when error occurs.
        /// Returns the <see cref="IDisposable"/> calling <see cref="IDisposable.Dispose"/> on which will cancel receiving.
        /// </summary>
        void ReceiveTextAll(Action<string> onReceived, Action<Exception> onError = null);

        /// <summary>
        /// Receiving a binary data.
        /// </summary>
        byte[] ReceiveBinary();

        /// <summary>
        /// Starts async receiving a binary data.
        /// Call the <paramref name="onReceived"/> when done.
        /// </summary>
        void ReceiveBinaryAsync(Action<byte[]> onReceived);

        /// <summary>
        /// Starts async receiving of all bynary packages.
        /// Call the <paramref name="onReceived"/> each time when package received.
        /// Call the <paramref name="onError"/> each time when error occurs.
        /// Returns the <see cref="IDisposable"/> calling <see cref="IDisposable.Dispose"/> on which will cancel receiving.
        /// </summary>
        void ReceiveBinaryAll(Action<byte[]> onReceived, Action<Exception> onError = null);

    }
}
