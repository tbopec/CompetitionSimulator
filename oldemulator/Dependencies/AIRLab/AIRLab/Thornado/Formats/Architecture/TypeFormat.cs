using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using AIRLab.Thornado.TypeFormats;
namespace AIRLab.Thornado
{
    /// <summary>
    /// Базовый класс для форматов ввода-вывода. Этот класс не типизирован, поэтому используется в тех случаях, когда обрабатываемый тип неизвестен. Это позволяет не протаскивать типизацию обрабатываемых объектов глубоко в код.
    /// Класс также содержит полезные статические методы, которые позволяют работать с форматами ввода-вывода.
    /// Как правило, не следует создавать непосредственных наследников этого типа.
    /// </summary>
    public abstract partial class TypeFormat
    {
        #region Базовая функциональность
        /// <summary>
        /// Записывает объект в строку. Пользователям не следует перегружать этот метод!
        /// </summary>
        public abstract string WriteObject(object obj);
        /// <summary>
        /// Парсит объект из строки. Пользователям не следует перегружать этот метод!
        /// </summary>
        public abstract object ParseObject(string str);
        /// <summary>
        /// Возвращает значение объекта по умолчанию. Пользователям не следует перегружать этот метод!
        /// </summary>
        public abstract object DefaultObject { get; }
        /// <summary>
        /// Возвращает тип объектов, которые парсит и записывает. Пользователям не следует перегружать этот метод!
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        /// Возвращает текстовое описание объектов, которые парсит и записывает (именительный падеж, с маленькой буквы, например "целое число", "дата" и т.д.)
        /// </summary>
        public abstract string Description { get; }
        #endregion
        #region Полезные мелочи
        /// <summary>
        /// Добавает строку нулями спереди, чтобы ее длина стала такой, как надо
        /// TODO: Это явный рак, переписать!
        /// </summary>
        public static string FillNumberString(int number, int length)
        {
            string s = number.ToString();
            while (s.Length < length) s = "0" + s;
            return s;
        }
    
        /// <summary>
        /// Пытается распарсить строку, и возвращает <see cref="DefaultObject"/> если ничего не получилось
        /// </summary>
        public object TryParseObjectOrDefault(string s)
        {
            return TryParseObjectOrDefault(s, DefaultObject);
        }

        /// <summary>
        /// Пытается распарсить строку, и возвращает <see cref="def"/> если ничего не получилось
        /// </summary>
        public object TryParseObjectOrDefault(string s, object def)
        {
            try
            {
                return ParseObject(s);
            }
            catch
            {
                return def;
            }
        }
        #endregion
        #region Определение формата по умолчанию
        /// <summary>
        /// Хэш доступных форматов по умолчанию
        /// </summary>
        static Dictionary<Type, TypeFormat> formats;
        /// <summary>
        /// Заполняет хэш доступных форматов по умолчанию
        /// </summary>
        static TypeFormat()
        {
            formats = new Dictionary<Type, TypeFormat>();
            var emp=new Type[]{};
            var ftypes = new List<Type>();
            
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                foreach (var z in asm.GetTypes())
                {
                    if (!z.IsSubclassOf(typeof(TypeFormat))
                        || z.IsAbstract
                        || z.GetConstructor(emp) == null
                        || z.IsGenericType) continue;
                    var attr = z.GetCustomAttributes(typeof(PrimaryFormatAttribute), false);
                    if (attr.Length == 0) continue;
                    ftypes.Add(z);

                }
            }
                
            foreach (var format in ftypes)
            {
                var baseFormat=format;
                while (true)
                {
                    baseFormat = baseFormat.BaseType;
                    if (baseFormat == null) break;
                    if (!baseFormat.IsGenericType) continue;
                    var baseDef=baseFormat.GetGenericTypeDefinition();
                    if (baseDef != typeof(BasicTypeFormat<>)) continue;
                    var type = baseFormat.GetGenericArguments()[0];
                    var attr = format.GetCustomAttributes(typeof(PrimaryFormatAttribute), false).Length;
                    if (!formats.Keys.Contains(type) || attr != 0)
                        formats[type] = (TypeFormat)format.GetConstructor(emp).Invoke(new object[] { });
                    break;
                }
            }
        }

        public static TypeFormat GetDefaultFormat(Type t)
        {
            if (t.IsEnum)
            {
                return (TypeFormat)typeof(EnumFormat<>).MakeGenericType(new Type[] { t }).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            }
            if (formats.ContainsKey(t))
                return formats[t];
            return null;
        }

        #endregion
    }

   
}