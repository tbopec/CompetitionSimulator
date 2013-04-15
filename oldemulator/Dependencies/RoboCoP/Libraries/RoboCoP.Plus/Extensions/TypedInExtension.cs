using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

using RoboCoP.Internal;

namespace RoboCoP.Plus
{

    public static  class TypedInExtension
    {

        private static ISerialiser Serialiser = new ThornadoSerialiser();

        /// <summary>
        /// Receiving a data of type <typeparamref name="T"/>.
        /// </summary>
        public static T ReceiveObject<T>(this IIn input)
        {
            try
            {
                return Serialiser.Deserialize<T>(input.ReceiveBinary());
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

        /// <summary>
        /// Starts async receiving a data of type <typeparamref name="T"/>.
        /// Call the <paramref name="onReceived"/> when done.
        /// </summary>
        public static void ReceiveObjectAsync<T>(this IIn input, Action<T> onReceived)
        {
            try
            {
                var receiveThread = new Thread(() => input.ReceiveBinaryAsync(data => onReceived(Serialiser.Deserialize<T>(data))));
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

        /// <summary>
        /// Starts async receiving of all packages with data of type <typeparamref name="T"/>.
        /// Call the <paramref name="onReceived"/> each time when data received.
        /// Call the <paramref name="onError"/> each time when error occurs.
        /// Returns the <see cref="IDisposable"/> calling <see cref="IDisposable.Dispose"/> on which will cancel receiving.
        /// </summary>
        public static void ReceiveObjectAll<T>(this IIn input, Action<T> onReceived, Action<Exception> onError = null)
        {
            try
            {
                var receiveThread = new Thread(() => input.ReceiveBinaryAll(data => onReceived(Serialiser.Deserialize<T>(data)), onError));
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

    }
}
