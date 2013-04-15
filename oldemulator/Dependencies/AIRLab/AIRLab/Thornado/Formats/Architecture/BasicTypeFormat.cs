using System;
namespace AIRLab.Thornado
{
    /// <summary>
    /// Класс типизированного формата ввода-вывода, который корректно обрабатывает null. Наследники класса <see cref="BasicTypeIO<T>"/> не способны перевести null в строку и считать его из строки. Для обработки значения null, необходимо использовать модификацию Nullable, которая определена в наследниках <see cref="BasicTypeIO<T>"/>: <see cref="ValueTypeIO<T>"/> и <see cref="ReferenceTypeIO<T>"/>
    /// Как правило, не следует создавать непосредственных наследников данного типа, и следует перегружать <see cref="ValueTypeIO<T>"/> (если обрабатываемый тип является типом-значением) и <see cref="ReferenceTypeIO<T>"/> (в противном случае). Эти классы, однако, отличаются лишь определением модификации Nullable. 
    /// Существует два способа работы создания формата ввода-вывода на основе <see cref="BasicTypeIO<T>"/>: <see cref="ValueTypeIO<T>"/> и <see cref="ReferenceTypeIO<T>"/>.
    /// 1) Неудобный простой метод: перегрузить <see cref="InternalParse"/>, <see cref="InternalWrite"/>, <see cref="InternalDefault"/> и <see cref="Description"/> и тем самым определить поведение класса.
    /// 2) Более сложный удобный метод: использовать конструктор, принимающй делегаты для преобразования в строку, парсинга, создания нового объекта и строку с описанием класса. Этот способ можно комбинировать с первым: если делегат парсинга слишком громоздкий, укажите null в конструкторе и перегрузите <see cref="InternalParse"/>. Делегаты никогда не могут получить null на вход, и не должны возвращать null.
    /// Кроме того, при выводе ReferenceTypeIO имеет смысл использовать {TODO:что именно?}, который позволяет удобно обрабатывать объекты, состоящие из многих полей.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicTypeFormat<T> : TypeFormat<T>
    {
        /// <summary>
        /// Делегат преобразования в строку
        /// </summary>
        Func<T, string> writeDelegate;
        /// <summary>
        /// Делегат парсинг
        /// </summary>
        Func<string, T> parseDelegate;
        /// <summary>
        /// Описание типа
        /// </summary>
        string description;

        /// <summary>
        /// Конструктор с делегатами. Используется во втором способе.
        /// </summary>
        protected BasicTypeFormat(Func<T, string> writeDelegate, Func<string, T> parseDelegate, string description)
        {
            this.writeDelegate = writeDelegate;
            this.parseDelegate = parseDelegate;
            this.description = description;
        }

        ///<inheritdoc/>
        public override string Description
        {
            get
            {
                return description;
            }
        }
 
        public override T Parse(string str)
        {
            if (str == null) throw new Exception("Строка не может быть равна null при парсинге через " + this.GetType().FullName);
            T obj = default(T);
            try
            {
                obj = parseDelegate(str);
            }
            catch(Exception e)
            {
                throw new FormatException("Некорректная строка '"+str+"' при парсинге через "+this.GetType().FullName+". Внутреннее исключение: "+e.Message);
            }
            if (obj == null) throw new Exception("Ошибка реализации " + GetType().FullName + ": Объект не может быть равен null при пасинге");
            return obj;
        }

          
       
        ///<inheritdoc/>
        public override string Write(T obj)
        {
            if (obj == null) throw new Exception("Объект не может равняться null при сохранении через " + this.GetType().FullName);
            string s = null;
            try
            {
                s = writeDelegate(obj);
            }
            catch(Exception e)
            {
                throw new FormatException("Некорректная объект при записи через " + this.GetType().FullName + ". Внутреннее исключение: " + e.Message);
            }
            if (s == null) throw new Exception("Ошибка реализации "+GetType().FullName+": Строка не может быть равна null при сохранении");
            return s;
        }
    }
}