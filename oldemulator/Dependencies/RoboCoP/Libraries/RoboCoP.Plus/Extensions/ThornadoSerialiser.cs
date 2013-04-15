using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

using System.Runtime.Serialization;

using System.Reflection;

using DCIMAP.Thornado;
using DCIMAP.Thornado.IOs;

namespace RoboCoP.Internal
{
    public class ThornadoSerialiser : ISerialiser
    {

        /// <summary>
        /// Contain predefined OBBs, that allow to write and to parse values of simple types.
        /// </summary>
        private IDictionary<Type, TypeIO> baseOBB = new Dictionary<Type, TypeIO>();

        /// <summary>
        /// Contain predefined OBBs, that allow to write and to parse arrays of simple types.
        /// </summary>
        private IDictionary<Type, TypeIO> baseArrayOBB = new Dictionary<Type, TypeIO>();


        public ThornadoSerialiser()
        {
            var baseOBBlist = ThornadoReflector.PredefinedOBB();

            foreach (var typeIOInstance in baseOBBlist)
            {
                if (!baseOBB.ContainsKey(typeIOInstance.Type))
                    baseOBB.Add(typeIOInstance.Type, typeIOInstance);

            }

            foreach (var baseType in baseOBB.Keys)
            {
                var arrayType = baseType.MakeArrayType();
                if (!baseArrayOBB.ContainsKey(arrayType))
                {
                    var genericTypeIO = typeof(TypeIO<>).MakeGenericType(new Type[] { baseType });
                    baseArrayOBB.Add(arrayType,
                        genericTypeIO.GetProperty("InArray").GetValue(baseOBB[baseType], new object[] { }) as TypeIO);
                }
            }

        }

        /// <inheritdoc/>
        public string Serialize<T>(T value)
        {
            Type type = typeof(T);

            if (baseOBB.ContainsKey(type))
                return BaseSerialise(value, baseOBB[type]);
            if (baseArrayOBB.ContainsKey(type))
                return BaseSerialise(value, baseArrayOBB[type]);

            return IniSerialise(value);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string data)
        {
            Type type = typeof(T);

            if (baseOBB.ContainsKey(type))
                return BaseDeserialise<T>(data, baseOBB[type]);
            if (baseArrayOBB.ContainsKey(type))
                return BaseDeserialise<T>(data, baseArrayOBB[type]);

            return IniDeserialise<T>(data);
        }


        /// <summary>
        /// Serialise a <paramref name="value"/> with predefined OBB.
        /// </summary>
        protected string BaseSerialise<T>(T value, TypeIO obb)
        {
            var type = typeof (T);
            if (!Equals(type, obb.Type))
                throw new ArgumentException("obb.Type != type");

            try
            {
                return obb.WriteObject(value);
            }
            catch (Exception error)
            {
                throw new SerializationException("Serialization has falled. Send a value as binary.", error);
            }
        }


        /// <summary>
        /// Deserialise a value with predefined OBB.
        /// </summary>
        protected T BaseDeserialise<T>(string data, TypeIO obb)
        {
            if (ReferenceEquals(data, null))
                throw new ArgumentNullException("data");
            Type type = typeof(T);
            if (!Equals(type, obb.Type))
                throw new ArgumentException("");

            try
            {
                return (T) obb.ParseObject(data);
            }
            catch (Exception error)
            {
                throw new SerializationException("Deserialization has falled. Received a data are incorrect.", error);
            }
        }


        /// <summary>
        /// Serialise a <paramref name="value"/> with a IniIO of <typeparamref name="T"/> object.
        /// </summary>
        protected string IniSerialise<T>(T value)
        {
            var type = typeof(T);
            if (!ThornadoReflector.IsIniWritable(type))
                throw new SerializationException("Serialization has falled. Add to type " + typeof(T) +
                                                 " the parameterless constructor.");

            string text;
            try
            {
                text = ThornadoReflector.WriteToIni(value);
            }
            catch (Exception error)
            {
                throw new SerializationException("Serialization has falled. Send a value as binary.", error);
            }
            if (String.IsNullOrEmpty(text))
                throw new SerializationException("Serialization has falled. Make type " + typeof(T) + " with thornado attributes.");

            return text;
        }

        /// <summary>
        /// Deserialise a value with a IniIO of <typeparamref name="T"/> object.
        /// </summary>
        protected T IniDeserialise<T>(string data)
        {
            if (ReferenceEquals(data, null))
                throw new ArgumentNullException("data");

            if (Equals(data, ""))
                throw new SerializationException("Deserialization has falled. Make type " + typeof (T) +
                                                 " with thornado attributes.");
            Type type = typeof(T);
            if (!ThornadoReflector.IsIniWritable(type))
                throw new SerializationException("Deserialization has falled. Add to type " + typeof(T) +
                                                 " the parameterless constructor.");
            T value = default(T);
            try
            {
                value = ThornadoReflector.ReadFromIni<T>(data);
            }
            catch (Exception error)
            {
                throw new SerializationException("Deserialization has falled. Received a data are incorrect.", error);
            }

            return value;
        }
    }
}
