using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using DCIMAP.Thornado;
using DCIMAP.Thornado.IOs;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Provide methods, that allow to get and to create thornado Input\Output Objects and to use they methods.
    /// </summary>
    public static class ThornadoReflector
    {

        /// <summary>
        /// Returns a list of OBB, that are predefined and are be contained in the TypeIO class. 
        /// </summary>
        public static IEnumerable<TypeIO> PredefinedOBB()
        {
           return typeof(TypeIO)
                .GetFields()
                .Where(info => info.IsPublic && info.IsStatic && !info.IsSpecialName)
                .Where(info => info.FieldType.IsSubclassOf(typeof(TypeIO)))
                .Select(info => info.GetValue(null) as TypeIO);
        }

        /// <summary>
        /// Verify, that a <paramref name="t"/> has parameterless constructor.
        /// </summary>
        public static bool IsIniWritable(Type t)
        {
            if (ReferenceEquals(t, null))
                throw new ArgumentNullException("t");

            return !ReferenceEquals(t.GetConstructor(new Type[] { }), null);
        }

        /// <summary>
        /// Write <paramref name="value"/> to a text of the ini format with object IniIO of <typeparamref name="T"/>.
        /// </summary>
        public static string WriteToIni<T>(T value)
        {
            Type valueType = typeof (T);
            if (!IsIniWritable(valueType))
                throw new ArgumentException(String.Format("Type {0} can not be write to the ini format.", valueType));

            var iniIOType = CreateIniIO(valueType); 
            var instance = CreateIniIOInstance(valueType);
            var writeMethod = iniIOType.GetMethod("WriteToString", new[] { valueType });
            return (string) writeMethod.Invoke(instance, new object[] {value});
        }

        /// <summary>
        /// Read value from <paramref name="text"/> of the ini format with object IniIO of <typeparamref name="T"/>.
        /// </summary>
        public static T ReadFromIni<T>(string text)
        {
            Type valueType = typeof(T);
            if (!IsIniWritable(valueType))
                throw new ArgumentException(String.Format("Type {0} can not be write to the ini format.", valueType));

            var iniIOType = CreateIniIO(valueType);
            var instance = CreateIniIOInstance(valueType);
            var parseMethod = iniIOType.GetMethod("ParseFromString", new[] { valueType, typeof(string) });
            var value = CreateParameterlessInstance<T>();

            parseMethod.Invoke(instance, new object[] {value, text});
            return value;
        }


        /// <summary>
        /// Create the generic type IniIO of <paramref name="t"/>.
        /// </summary>
        private static Type CreateIniIO(Type t)
        {
            return typeof (IniIO<>).MakeGenericType(new[] {t});
        }

        /// <summary>
        /// Create a instance of the generic type IniIO of <paramref name="t"/>.
        /// </summary>
        private static object CreateIniIOInstance(Type t)
        {
            return CreateIniIO(t).GetConstructor(new Type[] { }).Invoke(new object[] { });
        }

        /// <summary>
        /// Create a instance of the <typeparamref name="T"/> type, which provide parameterless constructor. 
        /// </summary>
        private static T CreateParameterlessInstance<T>()
        {
            return (T) typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
        }

    }
}
