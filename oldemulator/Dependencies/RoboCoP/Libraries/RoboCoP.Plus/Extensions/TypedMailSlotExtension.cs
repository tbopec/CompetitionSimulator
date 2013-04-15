using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization;

using RoboCoP.Internal;
using RoboCoP.Messages;

namespace RoboCoP.Plus
{
    /// <summary>
    /// Contain generic methods for communication with signals.
    /// </summary>
    public static class TypedMailSlotExtension
    {

        private static ISerialiser serialiser = new ThornadoSerialiser(); 

        /// <summary>
        /// Send a signal, that contains <paramref name="data"/>
        /// </summary>
        /// <typeparam name="T">type of data</typeparam>
        /// <exception cref="IOException">a data of the type <typeparamref name="T"/> does not serialize.</exception>
        public static void SendSignal<T>(this IMailSlot mail, string signalName, T data)
        {
            try
            {
                mail.SendSignal(signalName, serialiser.Serialize(data));
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }


        /// <summary>
        /// Synchronously receive an value of <typeparam name="T"/> type,
        /// with a signal that has <see cref="Signal.Name"/>==<paramref name="signalName"/>.
        /// </summary>
        /// <exception cref="IOException">Be throw when received signal contains a value of other type.</exception>
        public static T ReceiveSignal<T>(this IMailSlot mail, string signalName)
        {
            try
            {
                return serialiser.DeserializeFromString<T>(mail.ReceiveSignal(signalName));
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

        /// <summary>
        /// Add listener <param name="onReceive"/> which is invoked each time when value of <typeparam name="T"/> type
        /// with a signal that has  <see cref="Signal.Name"/>=<paramref name="signalName"/> received.
        /// If error has ocurrs during received, he ignored. If signal contains a value of other type 
        /// in his body listener does not invoke.
        /// </summary>
        public static void AddSignalListener<T>(this IMailSlot mail, string signalName, Action<T> onReceive)
        {
                mail.AddSignalListener(signalName, msg =>
                                                       {
                                                           T value = default(T);
                                                           try
                                                           {
                                                               value = serialiser.DeserializeFromString<T>(msg);
                                                           }
                                                           catch (SerializationException error)
                                                           {
                                                               throw new IOException(error.Message);
                                                           }
                                                           onReceive(value);
                                                       });
        }


    }

}
