using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace RoboCoP.Plus
{

    /// <summary>
    /// Declare methods, that convert values of any type to strings and contrariwise.
    /// </summary>
    /// <remarks>
    /// It allow transfer a typed value as body of robocop messages.
    /// </remarks>
    public interface ISerialiser
    {

        /// <summary>
        /// Convert <paramref name="value"/> of a type <typeparamref name="T"/> to a text data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="SerializationException"></exception>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Restore value of a <typeparamref name="T"/> type from a text <paramref name="data"/>.
        /// </summary>
        /// <exception cref="SerializationException"></exception>
        T Deserialize<T>(byte[] data);

    }

    public static class ISerializerExtensions
    {
        public static string SerializeToString<T>(this ISerialiser ser, T value)
        {
            return System.Text.Encoding.UTF8.GetString(ser.Serialize(value));
        }

        public static T DeserializeFromString<T>(this ISerialiser ser, string data)
    {
        return ser.Deserialize<T>(System.Text.Encoding.UTF8.GetBytes(data));
    }
    }
}
