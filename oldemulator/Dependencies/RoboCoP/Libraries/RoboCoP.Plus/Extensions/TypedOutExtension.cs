using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

using RoboCoP.Internal;

namespace RoboCoP.Plus
{
    public static class TypedOutExtension
    {

        private static ISerialiser Serialiser = new ThornadoSerialiser();

        /// <summary>
        /// Sends to output the <paramref name="data"/> of type <typeparamref name="T"/>.
        /// </summary>  
        public static void SendObject<T>(this IOut output, T data)
        {
            try
            {
                byte[] binary = Serialiser.Serialize(data);
                if (output is IDataOut)
                {
                    (output as IDataOut)
                        .SendBase(binary, new Dictionary<string, string>{{"content", data.GetType().ToString()}})
                        .Single();
                }
                else
                    output.SendBinary(binary);
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

        /// <summary>
        /// Start async sending the <paramref name="data"/> of type <typeparamref name="T"/>.
        /// </summary>
        public static void SendObjectAsync<T>(this IOut output, T data)
        {
            try
            {
                byte[] binary = Serialiser.Serialize(data);
                if (output is IDataOut)
                {
                    (output as IDataOut)
                        .SendBase(binary, new Dictionary<string, string> { { "content", data.GetType().ToString() } })
                        .Subscribe();
                }
                else
                    output.SendBinaryAsync(binary);
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

        /// <summary>
        /// Start async sending the <paramref name="data"/> of type <typeparamref name="T"/>.
        /// Call <paramref name="onSent"/> when done.
        /// </summary>
        public static void SendObjectAsync<T>(this IOut output, T data, Action onSent)
        {
            try
            {
                byte[] binary = Serialiser.Serialize(data);
                if (output is IDataOut)
                {
                    (output as IDataOut)
                        .SendBase(binary, new Dictionary<string, string> { { "content", data.GetType().ToString() } })
                        .Subscribe(_ => onSent());
                }
                else
                    output.SendBinaryAsync(binary, onSent);
            }
            catch (SerializationException error)
            {
                throw new IOException(error.Message);
            }
        }

    }
}
